using System;
using static Global.GlobalConstants;

namespace UIAccess.Models
{
    public class OptionModel
    {
        #region Private variables
        private UserSelectionOptions _Option;
        #endregion

        public string Description { get; set; }
        public string Label { get; private set; }
        public UserSelectionOptions Option
        {
            get
            {
                return _Option;
            }
            set
            {
                _Option = value;

                //Set the label
                Label = Enum.GetName(typeof(UserSelectionOptions), _Option);

            }
        }
    }
}
