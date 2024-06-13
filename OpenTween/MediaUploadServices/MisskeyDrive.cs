// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Linq;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Api.Misskey;
using OpenTween.SocialProtocol.Misskey;

namespace OpenTween.MediaUploadServices
{
    public class MisskeyDrive : IMediaUploadService
    {
        private readonly string[] pictureExt = { ".jpg", ".jpeg", ".gif", ".png" };

        private readonly MisskeyAccount account;

        public MisskeyDrive(MisskeyAccount account)
            => this.account = account;

        public int MaxMediaCount => 4;

        public string SupportedFormatsStrForDialog => "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";

        public bool CanUseAltText => true;

        public bool IsNativeUploadService => true;

        public bool CheckFileExtension(string fileExtension)
            => this.pictureExt.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase);

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
            => 3145728L; // 3MiB

        public async Task<PostStatusParams> UploadAsync(IMediaItem[] mediaItems, PostStatusParams postParams)
        {
            if (mediaItems == null)
                throw new ArgumentNullException(nameof(mediaItems));

            if (mediaItems.Length == 0)
                throw new ArgumentException("Err:Media not specified.");

            foreach (var item in mediaItems)
            {
                if (item == null)
                    throw new ArgumentException("Err:Media not specified.");

                if (!item.Exists)
                    throw new ArgumentException("Err:Media not found.");
            }

            var misskeyFileIds = await this.UploadDriveFiles(mediaItems)
                .ConfigureAwait(false);

            return postParams with { MediaIds = misskeyFileIds };
        }

        // pic.twitter.com の URL は文字数にカウントされない
        public int GetReservedTextLength(int mediaCount)
            => 0;

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
        {
        }

        private async Task<MisskeyFileId[]> UploadDriveFiles(IMediaItem[] mediaItems)
        {
            var uploadTasks = from m in mediaItems
                              select this.UploadMediaItem(m);

            var misskeyFileIds = await Task.WhenAll(uploadTasks)
                .ConfigureAwait(false);

            return misskeyFileIds;
        }

        private async Task<MisskeyFileId> UploadMediaItem(IMediaItem mediaItem)
        {
            var request = new DriveFileCreateRequest
            {
                File = mediaItem,
                Comment = mediaItem.AltText,
            };

            var misskeyFile = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            return new(misskeyFile.Id);
        }
    }
}
