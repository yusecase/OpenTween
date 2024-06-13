// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2024      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Twitter
{
    public class CreateTweetFormatter
    {
        private readonly TwitterAccount account;

        public CreateTweetFormatter(TwitterAccount account)
            => this.account = account;

        public int GetTextLengthRemain(PostStatusParams formState)
        {
            var createParams = this.CreateParams(formState);

            return this.account.Legacy.GetTextLengthRemain(createParams.Text);
        }

        public CreateTweetParams CreateParams(PostStatusParams formState)
        {
            var createParams = new CreateTweetParams(
                formState.Text,
                formState.InReplyTo,
                formState.MediaIds.Cast<TwitterMediaId>().ToArray()
            );

            // DM の場合はこれ以降の処理を行わない
            if (createParams.Text.StartsWith("D ", StringComparison.OrdinalIgnoreCase))
                return createParams;

            createParams = this.FormatStatusTextExtended(createParams, out var autoPopulatedUserIds);

            // リプライ先がセットされていても autoPopulatedUserIds が空の場合は auto_populate_reply_metadata を有効にしない
            //  (非公式 RT の場合など)
            if (createParams.InReplyTo != null && autoPopulatedUserIds.Length != 0)
            {
                createParams = createParams with
                {
                    AutoPopulateReplyMetadata = true,

                    // ReplyToList のうち autoPopulatedUserIds に含まれていないユーザー ID を抽出
                    ExcludeReplyUserIds = createParams.InReplyTo.ReplyToList.Select(x => x.UserId).Except(autoPopulatedUserIds)
                        .ToArray(),
                };
            }

            return createParams;
        }

        /// <summary>
        /// 拡張モードで140字にカウントされない文字列の除去を行います
        /// </summary>
        private CreateTweetParams FormatStatusTextExtended(CreateTweetParams createParams, out PersonId[] autoPopulatedUserIds)
        {
            createParams = this.RemoveAutoPopuratedMentions(createParams, out autoPopulatedUserIds);

            createParams = this.RemoveAttachmentUrl(createParams);

            return createParams;
        }

        /// <summary>
        /// 投稿時に auto_populate_reply_metadata オプションによって自動で追加されるメンションを除去します
        /// </summary>
        private CreateTweetParams RemoveAutoPopuratedMentions(CreateTweetParams createParams, out PersonId[] autoPopulatedUserIds)
        {
            autoPopulatedUserIds = Array.Empty<PersonId>();

            var replyToPost = createParams.InReplyTo;
            if (replyToPost == null)
                return createParams;

            var tweetText = createParams.Text;
            var autoPopulatedUserIdList = new List<PersonId>();

            if (tweetText.StartsWith($"@{replyToPost.ScreenName} ", StringComparison.Ordinal))
            {
                tweetText = tweetText.Substring(replyToPost.ScreenName.Length + 2);
                autoPopulatedUserIdList.Add(replyToPost.UserId);

                foreach (var (userId, screenName) in replyToPost.ReplyToList)
                {
                    if (tweetText.StartsWith($"@{screenName} ", StringComparison.Ordinal))
                    {
                        tweetText = tweetText.Substring(screenName.Length + 2);
                        autoPopulatedUserIdList.Add(userId);
                    }
                }
            }

            autoPopulatedUserIds = autoPopulatedUserIdList.ToArray();

            return createParams with { Text = tweetText };
        }

        /// <summary>
        /// attachment_url に指定可能な URL が含まれていれば除去
        /// </summary>
        private CreateTweetParams RemoveAttachmentUrl(CreateTweetParams createParams)
        {
            // attachment_url は media_id と同時に使用できない
            if (createParams.MediaIds.Count > 0)
                return createParams;

            var tweetText = createParams.Text;
            var match = OpenTween.Twitter.AttachmentUrlRegex.Match(tweetText);
            if (!match.Success)
                return createParams;

            var attachmentUrl = match.Value;

            // マッチした URL を空白に置換
            tweetText = tweetText.Substring(0, match.Index);

            // テキストと URL の間にスペースが含まれていれば除去
            tweetText = tweetText.TrimEnd(' ');

            return createParams with { Text = tweetText, AttachmentUrl = attachmentUrl };
        }
    }
}
