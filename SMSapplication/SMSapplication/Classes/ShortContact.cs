using System;
using System.Collections.Generic;
using System.Text;

namespace SMSapplication
{
	public class ShortContact
    {

        #region Private Variables
        private string number;
        private string name;
        private string id;
        private string national;
        private int index;
        #endregion

        #region Public Properties
        public string Number
		{
			get { return number;}
			set { number = value;}
		}

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public string National
        {
            get { return national; }
            set { national = value; }
        }

        #endregion

    }

    public class ShortContactCollection : List<ShortContact>
    {
    }
}
