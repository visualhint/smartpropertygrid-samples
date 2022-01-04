using System;
using System.Text;
using VisualHint.SmartPropertyGrid;
using System.Windows.Forms;

namespace WindowsApplication
{
    class PropertyValidatorFirstLetterUppercase : PropertyValidatorBase
    {
        public override bool Check(PropertyEnumerator propEnum, object value, bool modify)
        {
            if (value == null)
                return true;

            bool result = true;
            string str = value as string;

            try
            {
                result = Char.IsUpper(str, 0);

                if (modify && !result)
                {
                    str = Char.ToUpper(str[0]) + str.Substring(1);
                    propEnum.Property.Value.SetValue(str);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }

            if (result == false)
                Message = "The string must begin by an upper letter.";
            else
                Message = "";

            return result;
        }
    }
}
