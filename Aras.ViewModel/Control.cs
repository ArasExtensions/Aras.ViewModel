/*  
  Copyright 2017 Processwall Limited

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Web:     http://www.processwall.com
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Aras.ViewModel
{
    public enum Regions { Top=1, Bottom=2, Right=3, Left=4, Center=5, Leading=6, Trailing=7 };

    public abstract class Control : IEquatable<Control>, INotifyPropertyChanged
    {
        public Manager.Session Session { get; private set; }

        public Guid ID { get; private set; }

        private System.String _tooltip;
        [Attributes.Property("Tooltip", Attributes.PropertyTypes.String, true)]
        public System.String Tooltip
        {
            get
            {
                return this._tooltip;
            }
            set
            {
                if (this._tooltip == null)
                {
                    if (value != null)
                    {
                        this._tooltip = value;
                        this.OnPropertyChanged("Tooltip");
                    }
                }
                else
                {
                    if (!this._tooltip.Equals(value))
                    {
                        this._tooltip = value;
                        this.OnPropertyChanged("Tooltip");
                    }
                }
            }
        }

        [Attributes.Property("Height", Attributes.PropertyTypes.NullableInt32, true)]
        public System.Int32? Height { get; set; }

        [Attributes.Property("Width", Attributes.PropertyTypes.NullableInt32, true)]
        public System.Int32? Width { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String Name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(Name);
                this.PropertyChanged(this, args);
            }
        }

        private Boolean _enabled;
        [Attributes.Property("Enabled", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                if (this._enabled != value)
                {
                    this._enabled = value;
                    this.OnPropertyChanged("Enabled");
                }
            }
        }

        private String _errorMessage;
        [Attributes.Property("ErrorMessage", Attributes.PropertyTypes.String, true)]
        public String ErrorMessage
        {
            get
            {
                return this._errorMessage;
            }
            private set
            {
                if (this._errorMessage != value)
                {
                    this._errorMessage = value;
                    this.OnPropertyChanged("ErrorMessage");
                }
            }
        }

        protected void ResetError()
        {
            this.ErrorMessage = null;
        }

        protected void OnError(String ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        [ViewModel.Attributes.Property("Dialogs", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Dialog> Dialogs { get; private set; }

        private void Dialogs_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Dialogs");
        }

        protected virtual void CheckBinding(Object Binding)
        {

        }

        private Object _binding;
        public Object Binding
        {
            get
            {
                return this._binding;
            }
            set
            {
                if (this._binding == null)
                {
                    if (value != null)
                    {
                        this.CheckBinding(value);
                        this.BeforeBindingChanged();
                        this._binding = value;
                        this.AfterBindingChanged();
                        this.OnPropertyChanged("Binding");
                    }
                }
                else
                {
                    if (!this._binding.Equals(value))
                    {
                        this.CheckBinding(value);
                        this.BeforeBindingChanged();
                        this._binding = value;
                        this.AfterBindingChanged();
                        this.OnPropertyChanged("Binding");
                    }
                }
            }
        }

        protected virtual void AfterBindingChanged()
        {

        }

        protected virtual void BeforeBindingChanged()
        {

        }

        private Dictionary<String, System.Reflection.PropertyInfo> _commandInfoCache;
        private Dictionary<String, System.Reflection.PropertyInfo> CommandInfoCache
        {
            get
            {
                if (this._commandInfoCache == null)
                {
                    this._commandInfoCache = new Dictionary<String, System.Reflection.PropertyInfo>();

                    foreach (System.Reflection.PropertyInfo propinfo in this.GetType().GetProperties())
                    {
                        object[] customattrs = propinfo.GetCustomAttributes(typeof(Attributes.Command), true);

                        foreach (object customattr in customattrs)
                        {

                            this._commandInfoCache[((Attributes.Command)customattr).Name] = propinfo;
                            break;

                        }
                    }
                }

                return this._commandInfoCache;
            }
        }

        public IEnumerable<String> Commands
        {
            get
            {
                return this.CommandInfoCache.Keys;
            }
        }

        public Command GetCommand(String Name)
        {
            return (Command)this.CommandInfoCache[Name].GetValue(this);
        }

        private Dictionary<String, PropertyDetails> _propertyInfoCache;
        private readonly object _propertyInfoCacheLock = new object();
        private Dictionary<String, PropertyDetails> PropertyInfoCache
        {
            get
            {
                lock (this._propertyInfoCacheLock)
                {
                    if (this._propertyInfoCache == null)
                    {
                        this._propertyInfoCache = new Dictionary<String, PropertyDetails>();

                        foreach (System.Reflection.PropertyInfo propinfo in this.GetType().GetProperties())
                        {
                            object[] customattrs = propinfo.GetCustomAttributes(typeof(Attributes.Property), true);

                            foreach (object customattr in customattrs)
                            {
                                this._propertyInfoCache[((Attributes.Property)customattr).Name] = new PropertyDetails(propinfo, (Attributes.Property)customattr);
                                break;
                            }
                        }
                    }

                    return this._propertyInfoCache;
                }
            }
        }

        public IEnumerable<String> Properties
        {
            get
            {
                return this.PropertyInfoCache.Keys;
            }
        }

        public Boolean HasProperty(String Name)
        {
            return this.PropertyInfoCache.ContainsKey(Name);
        }

        public object GetPropertyValue(String Name)
        {
            return this.PropertyInfoCache[Name].PropertyInfo.GetValue(this);
        }

        public void SetPropertyValue(String Name, object value)
        {
            this.PropertyInfoCache[Name].PropertyInfo.SetValue(this, value);
        }

        public Boolean GetPropertyReadOnly(String Name)
        {
            return (this.PropertyInfoCache[Name].Attribute.ReadOnly);
        }

        public Attributes.PropertyTypes GetPropertyType(String Name)
        {
            return this.PropertyInfoCache[Name].Attribute.Type;
        }

        public IEnumerable<Control> Controls
        {
            get
            {
                List<Control> ret = new List<Control>();

                foreach (String property in this.Properties)
                {
                    object propertyvalue = this.GetPropertyValue(property);

                    if (propertyvalue is Control)
                    {
                        Control thiscontrol = (Control)propertyvalue;

                        if (!ret.Contains(thiscontrol))
                        {
                            ret.Add(thiscontrol);
                        }
                    }
                    else if (propertyvalue is IEnumerable<Control>)
                    {
                        foreach (Control thiscontrol in (IEnumerable<Control>)propertyvalue)
                        {
                            if (!ret.Contains(thiscontrol))
                            {
                                ret.Add(thiscontrol);
                            }
                        }
                    }
                }

                return ret;
            }
        }

        public bool Equals(Control other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.ID.Equals(other.ID);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj is Control)
                {
                    return this.ID.Equals(((Control)obj).ID);
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            return this.ID + " : " + this.GetType().FullName;
        }

        public Control(Manager.Session Session)
        {
            this.Session = Session;
            this.ID = Guid.NewGuid();
            this.ErrorMessage = null;
            this.Dialogs = new Model.ObservableList<Dialog>();
            this.Dialogs.ListChanged += Dialogs_ListChanged;

            // Add Control to Cache
            this.Session.CacheControl(this);
        }
    }
}
