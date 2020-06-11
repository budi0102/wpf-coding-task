using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WpfApp.Models
{
    public abstract class BindableBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);
            // CommandManager.InvalidateRequerySuggested();
            return true;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }


        #region IDataErrorInfo
        private readonly ObservableCollection<ModelError> validationErrors = new ObservableCollection<ModelError>();

        private void RemoveValidationErrors(string propertyName)
        {
            var propertyValidationErrors = validationErrors
                .Where(o => o.PropertyName.Equals(propertyName))
                .ToArray();
            foreach (var error in propertyValidationErrors)
            {
                validationErrors.Remove(error);
            }
        }

        private string ValidateProperty(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);

            RemoveValidationErrors(propertyName);

            var validationContext = new ValidationContext(this, null, null) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(property.GetValue(this), validationContext, validationResults))
            {
                foreach (var result in validationResults)
                {
                    validationErrors.Add(new ModelError(propertyName, result.ErrorMessage));
                }

                return validationResults[0].ErrorMessage;
            }

            return null;
        }

        public IEnumerable<ModelError> ValidationErrors
        {
            get { return this.validationErrors; }
        }

        public virtual string Error
        {
            get { return null; }
        }
        public string this[string columnName]
        {
            get { return ValidateProperty(columnName); }
        }
        #endregion
    }
}
