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

#nullable enable

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OpenTween.Api
{
    public class RateLimitCollection : IEnumerable<KeyValuePair<string, ApiLimit>>
    {
        public class AccessLimitUpdatedEventArgs : EventArgs
        {
            public string? EndpointName { get; }

            public AccessLimitUpdatedEventArgs(string? endpointName)
                => this.EndpointName = endpointName;
        }

        private event EventHandler<AccessLimitUpdatedEventArgs>? AccessLimitUpdated;

        private readonly ConcurrentDictionary<string, ApiLimit> innerDict = new();

        public ApiLimit? this[string endpoint]
        {
            get => this.innerDict.TryGetValue(endpoint, out var limit) ? limit : null;
            set
            {
                if (value == null)
                    this.innerDict.TryRemove(endpoint, out var _);
                else
                    this.innerDict[endpoint] = value;

                this.OnAccessLimitUpdated(new(endpoint));
            }
        }

        public void Clear()
        {
            this.innerDict.Clear();
            this.OnAccessLimitUpdated(new(null));
        }

        public void AddAll(IDictionary<string, ApiLimit> resources)
        {
            foreach (var (key, value) in resources)
            {
                this.innerDict[key] = value;
            }

            this.OnAccessLimitUpdated(new(null));
        }

        public IDisposable SubscribeAccessLimitUpdated(Action<RateLimitCollection, AccessLimitUpdatedEventArgs> action)
        {
            void Handler(object sender, AccessLimitUpdatedEventArgs e)
                => action((RateLimitCollection)sender, e);

            this.AccessLimitUpdated += Handler;

            void Unsubscribe()
                => this.AccessLimitUpdated -= Handler;

            return new Unsubscriber(Unsubscribe);
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly Action action;

            public Unsubscriber(Action action)
                => this.action = action;

            public void Dispose()
                => this.action();
        }

        protected virtual void OnAccessLimitUpdated(AccessLimitUpdatedEventArgs e)
            => this.AccessLimitUpdated?.Invoke(this, e);

        public IEnumerator<KeyValuePair<string, ApiLimit>> GetEnumerator()
            => this.innerDict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
