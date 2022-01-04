using System;
using VisualHint.SmartPropertyGrid;
using System.Drawing;

namespace WindowsApplication
{
    public class PropertyValidatorPointFMinMax : PropertyValidatorBase
    {
        private float _min1;

        private float _max1;

        private float _min2;

        private float _max2;

        public PropertyValidatorPointFMinMax(float min1, float max1, float min2, float max2)
        {
            _min1 = min1;
            _max1 = max1;
            _min2 = min2;
            _max2 = max2;
        }

        public override bool Check(PropertyEnumerator propEnum, object value, bool modify)
        {
            if (value == null)
                return true;

            if (propEnum == null)
                throw new NullReferenceException("The constructor of the validator should have been passed an enumerator on the corresponding property.");

            bool result = true;
            float valueToCheck = ((PointF)value).X;

            if (valueToCheck < _min1)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_min1);
                result = false;
            }
            else if (valueToCheck > _max1)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_max1);
                result = false;
            }

            if (result == false)
            {
                Message = "" + valueToCheck + " is not between " + _min1 + " and " + _max1 + ".";
                return false;
            }
            else
                Message = "";

            valueToCheck = ((PointF)value).Y;

            if (valueToCheck < _min2)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_min2);
                result = false;
            }
            else if (valueToCheck > _max2)
            {
                if (modify)
                    propEnum.Property.Value.SetValue(_max2);
                result = false;
            }

            if (result == false)
            {
                Message = "" + valueToCheck + " is not between " + _min2 + " and " + _max2 + ".";
                return false;
            }
            else
                Message = "";

            return true;
        }

        public override int GetHashCode()
        {
            return _min1.GetHashCode() ^ _max1.GetHashCode() ^ _min2.GetHashCode() ^ _max2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PropertyValidatorPointFMinMax validator = obj as PropertyValidatorPointFMinMax;
            if (validator == null)
                return false;

            return (_min1.Equals(validator._min1) && _max1.Equals(validator._max1) &&
                _min2.Equals(validator._min2) && _max2.Equals(validator._max2));
        }
    }
}
