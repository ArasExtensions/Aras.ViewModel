/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel
{
    public class ObservableList<T> : List<T>
    {
        private int NotifyCount;
        private Boolean _notifiyListChanged;
        public Boolean NotifyListChanged
        {
            get
            {
                return this._notifiyListChanged;
            }
            set
            {
                if (value)
                {
                    if (!this._notifiyListChanged)
                    {
                        this._notifiyListChanged = true;

                        if (this.NotifyCount > 0)
                        {
                            this.OnListChanged();
                        }
                    }
                }
                else
                {
                    if (this._notifiyListChanged)
                    {
                        this._notifiyListChanged = false;
                        this.NotifyCount = 0;
                    }
                }
            }
        }

        public event EventHandler ListChanged;

        private void OnListChanged()
        {
            if (this.ListChanged != null)
            {
                if (this._notifiyListChanged)
                {
                    this.ListChanged(this, EventArgs.Empty);
                }
                else
                {
                    this.NotifyCount++;
                }
            }
        }

        public new void Add(T item)
        {
            base.Add(item);
            this.OnListChanged();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            this.OnListChanged();
        }

        public new void Clear()
        {
            if (this.Count > 0)
            {
                base.Clear();
                this.OnListChanged();
            }
        }

        public new bool Remove(T item)
        {
            bool ret = base.Remove(item);
            this.OnListChanged();
            return ret;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int ret = base.RemoveAll(match);
            this.OnListChanged();
            return ret;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            this.OnListChanged();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.OnListChanged();
        }

        public new void Reverse()
        {
            base.Reverse();
            this.OnListChanged();
        }

        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            this.OnListChanged();
        }

        public new void Sort()
        {
            base.Sort();
            this.OnListChanged();
        }

        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            this.OnListChanged();
        }

        public new void Sort(IComparer<T> comparer)
        {
            base.Sort();
            this.OnListChanged();
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            this.OnListChanged();
        }
    }
}
