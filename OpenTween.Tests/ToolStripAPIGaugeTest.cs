// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTween.Api;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class ToolStripAPIGaugeTest
    {
        [Fact]
        public void ApiEndpointTest()
        {
            using var toolStrip = new TestToolStripAPIGauge();
            var now = DateTimeUtc.Now;
            toolStrip.DateTimeNow = now;

            // toolStrip.ApiEndpoint の初期値は null
            Assert.Null(toolStrip.ApiEndpoint);
            Assert.Null(toolStrip.ApiLimit);

            toolStrip.ApiEndpoint = "endpoint1";
            toolStrip.ApiLimit = new(15, 15, now + TimeSpan.FromMinutes(15));

            Assert.Equal("endpoint1", toolStrip.ApiEndpoint);
            Assert.Equal(new ApiLimit(15, 15, now + TimeSpan.FromMinutes(15)), toolStrip.ApiLimit);

            toolStrip.ApiEndpoint = "endpoint2";
            toolStrip.ApiLimit = new(180, 18, now + TimeSpan.FromMinutes(5));

            Assert.Equal("endpoint2", toolStrip.ApiEndpoint);
            Assert.Equal(new ApiLimit(180, 18, now + TimeSpan.FromMinutes(5)), toolStrip.ApiLimit);

            toolStrip.ApiLimit = new(180, 17, now + TimeSpan.FromMinutes(5));

            Assert.Equal("endpoint2", toolStrip.ApiEndpoint);
            Assert.Equal(new ApiLimit(180, 17, now + TimeSpan.FromMinutes(5)), toolStrip.ApiLimit);

            toolStrip.ApiEndpoint = "hoge";
            toolStrip.ApiLimit = null;

            Assert.Equal("hoge", toolStrip.ApiEndpoint);
            Assert.Null(toolStrip.ApiLimit);

            toolStrip.ApiEndpoint = null;

            Assert.Null(toolStrip.ApiEndpoint);
            Assert.Null(toolStrip.ApiLimit);
        }

        [Fact]
        public void GaugeHeightTest()
        {
            using var toolStrip = new ToolStripAPIGauge();
            toolStrip.AutoSize = false;
            toolStrip.Size = new Size(100, 10);

            toolStrip.ApiLimit = new(15, 15, DateTimeUtc.MaxValue);
            toolStrip.GaugeHeight = 5;

            Assert.Equal(new Rectangle(0, 0, 100, 5), toolStrip.ApiGaugeBounds);
            Assert.Equal(new Rectangle(0, 5, 100, 5), toolStrip.TimeGaugeBounds);

            toolStrip.GaugeHeight = 3;

            Assert.Equal(new Rectangle(0, 2, 100, 3), toolStrip.ApiGaugeBounds);
            Assert.Equal(new Rectangle(0, 5, 100, 3), toolStrip.TimeGaugeBounds);

            toolStrip.GaugeHeight = 0;

            Assert.Equal(Rectangle.Empty, toolStrip.ApiGaugeBounds);
            Assert.Equal(Rectangle.Empty, toolStrip.TimeGaugeBounds);
        }

        [Fact]
        public void TextTest()
        {
            using var toolStrip = new ToolStripAPIGauge();

            // toolStrip.ApiEndpoint の初期値は null
            Assert.Equal("API ???/???", toolStrip.Text);
            Assert.Equal("API rest unknown ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

            toolStrip.ApiEndpoint = "/search/tweets";
            toolStrip.ApiLimit = new(180, 90, DateTimeUtc.Now + TimeSpan.FromMinutes(5));

            Assert.Equal("API 90/180", toolStrip.Text);
            Assert.Equal("API rest /search/tweets 90/180" + Environment.NewLine + "(reset after 5 minutes)", toolStrip.ToolTipText);

            toolStrip.ApiEndpoint = "/statuses/user_timeline";
            toolStrip.ApiLimit = new(180, 18, DateTimeUtc.Now + TimeSpan.FromMinutes(-2));

            Assert.Equal("API ???/???", toolStrip.Text);
            Assert.Equal("API rest /statuses/user_timeline ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

            toolStrip.ApiLimit = new(180, 180, DateTimeUtc.Now + TimeSpan.FromMinutes(15));

            Assert.Equal("API 180/180", toolStrip.Text);
            Assert.Equal("API rest /statuses/user_timeline 180/180" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);

            toolStrip.ApiLimit = new(180, 179, DateTimeUtc.Now + TimeSpan.FromMinutes(15));

            Assert.Equal("API 179/180", toolStrip.Text);
            Assert.Equal("API rest /statuses/user_timeline 179/180" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);

            toolStrip.ApiEndpoint = "hoge";
            toolStrip.ApiLimit = null;

            Assert.Equal("API ???/???", toolStrip.Text);
            Assert.Equal("API rest hoge ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);

            toolStrip.ApiEndpoint = null;

            Assert.Equal("API ???/???", toolStrip.Text);
            Assert.Equal("API rest unknown ???/???" + Environment.NewLine + "(reset after ??? minutes)", toolStrip.ToolTipText);
        }

        private class TestToolStripAPIGauge : ToolStripAPIGauge
        {
            public DateTimeUtc DateTimeNow = DateTimeUtc.Now;

            protected override void UpdateRemainMinutes()
            {
                if (this.ApiLimit != null)
                    // DateTime.Now の代わりに this.DateTimeNow を使用することで任意の日時のテストができるようにする
                    this.remainMinutes = (this.ApiLimit.AccessLimitResetDate - this.DateTimeNow).TotalMinutes;
                else
                    this.remainMinutes = -1;
            }
        }

        [Fact]
        public void GaugeBoundsTest()
        {
            using var toolStrip = new TestToolStripAPIGauge();

            var now = DateTimeUtc.Now;
            toolStrip.DateTimeNow = now;

            toolStrip.AutoSize = false;
            toolStrip.Size = new Size(100, 10);
            toolStrip.GaugeHeight = 5;

            // toolStrip.ApiEndpoint の初期値は null

            Assert.Equal(Rectangle.Empty, toolStrip.ApiGaugeBounds);
            Assert.Equal(Rectangle.Empty, toolStrip.TimeGaugeBounds);

            toolStrip.ApiLimit = new(150, 60, now + TimeSpan.FromMinutes(3));

            Assert.Equal(new Rectangle(0, 0, 40, 5), toolStrip.ApiGaugeBounds); // 40% (60/150)
            Assert.Equal(new Rectangle(0, 5, 20, 5), toolStrip.TimeGaugeBounds); // 20% (3/15)

            toolStrip.ApiLimit = null;

            Assert.Equal(Rectangle.Empty, toolStrip.ApiGaugeBounds);
            Assert.Equal(Rectangle.Empty, toolStrip.TimeGaugeBounds);
        }

        [Fact]
        public void OneBillionTest()
        {
            using var toolStrip = new TestToolStripAPIGauge();

            var now = new DateTimeUtc(2020, 2, 25, 19, 0, 0);
            toolStrip.DateTimeNow = now;

            toolStrip.AutoSize = false;
            toolStrip.Size = new Size(100, 10);
            toolStrip.GaugeHeight = 5;

            toolStrip.ApiLimit = new(
                AccessLimitCount: 1_000_000_000,
                AccessLimitRemain: 999_999_999,
                AccessLimitResetDate: now + TimeSpan.FromMinutes(15)
            );
            toolStrip.ApiEndpoint = "/statuses/user_timeline";

            Assert.Equal(new Rectangle(0, 0, 99, 5), toolStrip.ApiGaugeBounds); // 99% (999999999/1000000000)
            Assert.Equal(new Rectangle(0, 5, 100, 5), toolStrip.TimeGaugeBounds); // 100% (15/15)
            Assert.Equal("API 999999999/1000000000", toolStrip.Text);
            Assert.Equal("API rest /statuses/user_timeline 999999999/1000000000" + Environment.NewLine + "(reset after 15 minutes)", toolStrip.ToolTipText);
        }
    }
}
