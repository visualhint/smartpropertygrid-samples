using System;
using VisualHint.SmartPropertyGrid;
using System.Drawing;

namespace WindowsApplication
{
    public class PropertyValidatorFontSize : PropertyValidatorBase
    {
        private float _min;

        private float _max;

        public PropertyValidatorFontSize(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public override bool Check(PropertyEnumerator propEnum, object value, bool modify)
        {
            if (value == null)
                return true;

            if (propEnum == null)
                throw new NullReferenceException("The constructor of the validator should have been passed an enumerator on the corresponding property.");

            bool result = true;
            float valueToCheck = ((Font)value).Size;

            if (valueToCheck < _min)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_min);
                result = false;
            }
            else if (valueToCheck > _max)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_max);
                result = false;
            }

            if (result == false)
            {
                Message = "" + valueToCheck + " is not between " + _min + " and " + _max + ".";
                return false;
            }
            else
                Message = "";

            return true;
        }

        public override int GetHashCode()
        {
            return _min.GetHashCode() ^ _max.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PropertyValidatorFontSize validator = obj as PropertyValidatorFontSize;
            if (validator == null)
                return false;

            return (_min.Equals(validator._min) && _max.Equals(validator._max));
        }
    }
}
