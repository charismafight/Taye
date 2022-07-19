using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Taye.Models;
using System.Net.Http.Headers;
using Taye.Repositories;
using Taye.Utilities;
using Microsoft.EntityFrameworkCore;
using Taye.Enums;
using Taye.Common;
using System;

namespace Taye.Pages.Admin.Archives
{
    public class FormItemLayout
    {
        public ColLayoutParam LabelCol { get; set; }
        public ColLayoutParam WrapperCol { get; set; }
    }

    public partial class ArchiveDetail : BaseRazor
    {
        [Parameter]
        public int? Id { get; set; }

        public Archive? Archive { get; set; }

        [Inject]
        TayeContext Context { get; set; }

        [Inject]
        NavigationManager NavManager { get; set; }

        [Inject]
        FileHelper Uploader { get; set; }

        private readonly FormItemLayout _formItemLayout = new FormItemLayout
        {
            LabelCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 7 },
            },

            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24 },
                Sm = new EmbeddedProperty { Span = 12 },
                Md = new EmbeddedProperty { Span = 10 },
            }
        };

        private readonly FormItemLayout _submitFormLayout = new FormItemLayout
        {
            WrapperCol = new ColLayoutParam
            {
                Xs = new EmbeddedProperty { Span = 24, Offset = 0 },
                Sm = new EmbeddedProperty { Span = 10, Offset = 7 },
            }
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Id.HasValue)
            {
                Archive = Context.Archives
                    .Include(archive => archive.MediaFile)
                    .SingleOrDefault(o => o.Id == Id.Value);
            }
            else
            {
                Id = 0;
            }

            var testUser = Context.Users.Single(o => o.Id == 1);

            if (Archive == null)
            {
                Archive = new Archive() { Author = testUser };
                Context.Archives.Add(Archive);
            }
            else
            {
                PreviewFileFullName = Archive.MediaFile?.FilePath;
                PreviewThumbnailFullName = Archive.MediaFile?.ThumbnailFilePath;
            }
        }

        IBrowserFile UploadedFile;
        bool shouldRender;
        string UploadFileInfo = string.Empty;
        string UploadFileFullName = string.Empty;
        string UploadFileUrl = string.Empty;
        string UploadFileMd5 = string.Empty;
        string UploadFileMd5Name = string.Empty;
        string UploadFileMd5ThumbName = string.Empty;
        string PreviewFileFullName = string.Empty;
        string PreviewThumbnailFullName = string.Empty;

        protected override bool ShouldRender() => shouldRender;

        Stream fs;
        async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            long maxFileSize = 1024 * 1024 * 15;

            using var content = new MultipartFormDataContent();

            UploadedFile = e.File;

            if (UploadedFile.Name.EndsWith(Constants.ImageFileSufix) || UploadedFile.Name.EndsWith(Constants.VideoFileSufix))
            {
                try
                {
                    var FileContent = new StreamContent(UploadedFile.OpenReadStream(maxFileSize));
                    fs = await FileContent.ReadAsStreamAsync();
                    FileContent.Headers.ContentType = new MediaTypeHeaderValue(UploadedFile.ContentType);
                    content.Add(
                        content: FileContent,
                        name: "\"file\"",
                        fileName: UploadedFile.Name);
                    //直接上传文件
                    UploadFileInfo = UploadedFile.Name;
                    UploadFileFullName = await Uploader.UploadFile(fs, UploadedFile.Name, "media");
                    UploadFileMd5 = await FileHelper.CalculateMD5Async(UploadFileFullName);
                    UploadFileMd5Name = string.Concat(UploadFileMd5, UploadedFile.Name.AsSpan(UploadedFile.Name.LastIndexOf('.')));
                    UploadFileMd5ThumbName = Constants.ThumbPrefix + UploadFileMd5Name;
                    //确认文件是否有重复
                    if (FileHelper.FileExists(Constants.MediaFullPath, UploadFileMd5Name))
                    {
                        //重复就移除上传的这个文件
                        FileHelper.RemoveFile(UploadFileFullName);
                    }
                    else
                    {
                        //不重复就rename成md码+后缀形式
                        FileHelper.Rename(UploadFileFullName, UploadFileInfo, UploadFileMd5Name);
                    }

                    PreviewFileFullName = Path.Combine(Constants.MediaDirectory, UploadFileMd5Name);

                    await GenerateThumbnail(PreviewFileFullName);

                    shouldRender = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await ShowFail();
                }
            }
            else
            {
                await ShowFail();
            }
        }

        async Task GenerateThumbnail(string fileName)
        {
            PreviewThumbnailFullName = Path.Combine(Constants.MediaDirectory, UploadFileMd5ThumbName);
            if (fileName.EndsWith(Constants.ImageFileSufix))
            {
                //构造文件缩略图
                ImageHelper.CompressImgFile(200, 200, Path.Combine(Constants.FileRootPath, PreviewFileFullName), Path.Combine(Constants.FileRootPath, PreviewThumbnailFullName));
            }
            else if (fileName.EndsWith(Constants.VideoFileSufix))
            {
                //.mp4->png
                PreviewThumbnailFullName = PreviewThumbnailFullName.Replace(".mp4", ".jpg");
                await VideoHelper.GetFrame(Path.Combine(Constants.FileRootPath, PreviewFileFullName), PreviewThumbnailFullName);
            }
        }

        private async void HandleSubmit()
        {
            if (UploadedFile == null)
            {
                await ShowFail("必须上传照片或视频后提交");
                return;
            }

            DoWithLoading(() =>
            {
                Thread.Sleep(10000);
                SaveArchive();
                Context.SaveChanges();
            });

            await ShowSuccess();
            NavManager.NavigateTo("/admin/archive");
        }

        void SaveArchive()
        {
            //判断重复，相同文件直接修改指向
            if (Context.UploadFiles.Any(p => p.MD5 == UploadFileMd5))
            {
                Archive.MediaFile = Context.UploadFiles.Single(o => o.MD5 == UploadFileMd5);
            }
            else
            {
                Archive.MediaFile = new UploadFile()
                {
                    MD5 = UploadFileMd5,
                    Name = UploadedFile.Name,
                    FilePath = Path.Combine(Constants.MediaDirectory, UploadFileMd5Name),
                    FileType = UploadedFile.Name.EndsWith(Constants.ImageFileSufix) ? FileType.Image : FileType.Video,
                    Size = UploadedFile.Size,
                    ThumbnailFilePath = PreviewThumbnailFullName
                };
            }
        }

        void BackToList()
        {
            NavManager.NavigateTo("/admin/archive");
        }
    }
}
