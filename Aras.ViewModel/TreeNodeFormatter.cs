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
    public abstract class TreeNodeFormatter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String Name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(Name);
                this.PropertyChanged(this, args);
            }
        }

        public TreeNode Node { get; private set; }

        public abstract String Label { get; }

        public abstract String OpenIcon { get; }

        public abstract String ClosedIcon { get; }


        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Binding":
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Label"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("OpenIcon"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ClosedIcon"));
                    break;
                default:
                    break;
            }
        }

        public TreeNodeFormatter(TreeNode Node)
        {
            this.Node = Node;
            this.Node.PropertyChanged += Node_PropertyChanged;
        }
    }
}
