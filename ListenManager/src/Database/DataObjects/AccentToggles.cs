﻿namespace ListenManager.Database.DataObjects
{
    public class AccentToggles : BaseDataObject
    {
        private bool _isChecked;
        private string _name;
        private string _displayName;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is AccentToggles toggles && Name.Equals(toggles.Name);
        }

        protected bool Equals(AccentToggles other)
        {
            return string.Equals(Name, other._name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
