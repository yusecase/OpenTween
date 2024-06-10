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
using System.Collections.Generic;
using System.Linq;
using OpenTween.Api.DataModel;
using OpenTween.Api.Misskey;
using OpenTween.Models;
using OpenTween.Setting;
using OpenTween.Thumbnail;

namespace OpenTween.SocialProtocol.Misskey
{
    public class MisskeyPostFactory
    {
        private readonly SettingCommon settingCommon;

        public MisskeyPostFactory()
            : this(SettingManager.Instance.Common)
        {
        }

        public MisskeyPostFactory(SettingCommon settingCommon)
            => this.settingCommon = settingCommon;

        public PostClass CreateFromNote(MisskeyNote note, MisskeyAccountState accountState, bool firstLoad)
        {
            var noteUser = note.User;
            var noteUserId = new MisskeyUserId(noteUser.Id);

            MisskeyNote? renotedNote = null;
            MisskeyUserLite? renoterUser = null;
            PostId[] quotedNoteIds = Array.Empty<PostId>();

            if (note.Renote != null)
            {
                if (MyCommon.IsNullOrEmpty(note.Text))
                {
                    renotedNote = note.Renote;
                    renoterUser = noteUser;
                }
                else
                {
                    quotedNoteIds = new PostId[] { new MisskeyNoteId(note.Renote.Id) };
                }
            }

            // リツイートであるか否かに関わらず常にオリジナルのツイート及びユーザーを指す
            var originalNote = renotedNote ?? note;
            var originalNoteId = new MisskeyNoteId(originalNote.Id);
            var originalNoteUser = originalNote.User;
            var originalNoteUserId = new MisskeyUserId(originalNoteUser.Id);
            var originalNoteUserAcct = originalNoteUser.Host is { } host ? $"{originalNoteUser.Username}@{host}" : originalNoteUser.Username;

            var replyToNote = originalNote.Reply;

            var isMe = noteUserId == accountState.UserId;
            var reactionSent = note.MyReaction != null;

            var originalText = originalNote.Text ?? "";
            var urlEntities = TweetExtractor.ExtractUrlEntities(originalText);
            var textHtml = TweetFormatter.AutoLinkHtml(originalText, urlEntities);

            var quotedNoteIdsInText = this.ExtractPostIdFromUrls(urlEntities).ToArray();
            if (quotedNoteIdsInText.Length > 0)
                quotedNoteIds = Enumerable.Concat(quotedNoteIdsInText, quotedNoteIds).ToArray();

            var textSuffix = "";
            if (originalNote.Files.Length > 0)
                textSuffix += " [画像]";
            if (originalNote.Renote != null && !MyCommon.IsNullOrEmpty(originalNote.Text))
                textSuffix += " [引用]";

            if (!MyCommon.IsNullOrEmpty(textSuffix))
            {
                if (MyCommon.IsNullOrEmpty(originalText))
                    originalText = textSuffix.TrimStart(' ');
                else
                    originalText += textSuffix;
            }

            return new()
            {
                // note から生成
                StatusId = new MisskeyNoteId(note.Id),
                CreatedAtForSorting = DateTimeUtc.ParseISO(note.CreatedAt),
                IsMe = isMe,

                // originalNote から生成
                PostUri = CreateLocalPostUri(accountState.ServerUri, originalNoteId),
                CreatedAt = DateTimeUtc.ParseISO(originalNote.CreatedAt),
                Text = textHtml,
                TextFromApi = originalText,
                AccessibleText = originalText,
                QuoteStatusIds = quotedNoteIds,
                PreloadedThumbnails = this.CreateThumbnailInfoList(originalNote.Files),
                IsFav = reactionSent,
                IsReply = renotedNote == null && originalNote.Mentions?.Any(x => x == accountState.UserId.Id) == true,
                InReplyToStatusId = replyToNote?.Id is { } replyToIdStr ? new MisskeyNoteId(replyToIdStr) : null,
                InReplyToUser = replyToNote?.User.Username,
                InReplyToUserId = replyToNote?.UserId is { } replyToUserIdStr ? new MisskeyUserId(replyToUserIdStr) : null,
                IsProtect = originalNote.Visibility is not "public" or "home",

                // originalNoteUser から生成
                UserId = originalNoteUserId,
                ScreenName = originalNoteUserAcct,
                Nickname = originalNoteUser.Name ?? originalNoteUser.Username,
                ImageUrl = originalNoteUser.AvatarUrl ?? "",

                // renotedNote から生成
                RetweetedId = renotedNote?.Id is { } renotedId ? new MisskeyNoteId(renotedId) : null,

                // renoterUser から生成
                RetweetedBy = renoterUser?.Username,
                RetweetedByUserId = renoterUser?.Id is { } renoterId ? new MisskeyUserId(renoterId) : null,

                IsRead = this.DetermineUnreadState(isMe, firstLoad),
            };
        }

        private IEnumerable<PostId> ExtractPostIdFromUrls(IEnumerable<TwitterEntityUrl> urls)
        {
            foreach (var url in urls)
            {
                var match = OpenTween.Twitter.StatusUrlRegex.Match(url.ExpandedUrl);
                if (match.Success)
                    yield return new TwitterStatusId(match.Groups["StatusId"].Value);
            }
        }

        public static Uri CreateLocalPostUri(Uri serverUri, MisskeyNoteId noteId)
            => new(serverUri, $"/notes/{noteId.Id}");

        private ThumbnailInfo[] CreateThumbnailInfoList(MisskeyDriveFile[] files)
        {
            var thumbnails = new List<ThumbnailInfo>(capacity: files.Length);

            foreach (var file in files)
            {
                if (MyCommon.IsNullOrEmpty(file.ThumbnailUrl))
                    continue;

                var thumb = new ThumbnailInfo(file.Url, file.ThumbnailUrl)
                {
                    TooltipText = file.Comment ?? "",
                    IsPlayable = file.Type.StartsWith("video/"),
                };

                thumbnails.Add(thumb);
            }

            return thumbnails.ToArray();
        }

        private bool DetermineUnreadState(bool isMe, bool firstLoad)
        {
            if (isMe && this.settingCommon.ReadOwnPost)
                return true;

            if (firstLoad && this.settingCommon.Read)
                return true;

            return false;
        }
    }
}
