using System;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace WindowsApplication
{
    public class MyTypeConverter : TypeConverter
    {
        public class MyPropertyDescriptor : PropertyDescriptor
        {
            public class MyStringConverter : BooleanConverter
            {
                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return true;
                }
            }

            public MyPropertyDescriptor(string name, Attribute[] attributes, bool value)
                : base(name, attributes)
            {
                _value = value;
            }

            private TypeConverter _converter = new MyStringConverter();

            private bool _value;

            public override bool IsReadOnly { get { return false; } }

            public override void ResetValue(object component) { }

            public override bool CanResetValue(object component) { return false; }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(MyPropertyDescriptor); }
            }

            public override Type PropertyType { get { return typeof(bool); } }

            public override object GetValue(object component)
            {
                return _value;
            }

            public override void SetValue(object component, object value)
            {
                _value = (bool)value;
                OnValueChanged(component, EventArgs.Empty);
            }

            public override TypeConverter Converter
            {
                get
                {
                    return _converter;
                }
            }

            public override string Description
            {
                get
                {
                    return null;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return base.DisplayName;
                }
            }
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            ArrayList attributesList = new ArrayList();
            attributesList.Add(new VisualHint.SmartPropertyGrid.PropertyFeelAttribute(
                VisualHint.SmartPropertyGrid.PropertyGrid.FeelCheckbox));

            attributesList.Add(new VisualHint.SmartPropertyGrid.PropertyLookAttribute(
                typeof(VisualHint.SmartPropertyGrid.PropertyCheckboxLook)));

            attributesList.Add(new VisualHint.SmartPropertyGrid.PropertyValueDisplayedAsAttribute(
                new string[] {"yes","no"}));

            Attribute[] attrArray = (Attribute[])attributesList.ToArray(typeof(Attribute));

            MyPropertyDescriptor[] pds = new MyPropertyDescriptor[1];
            pds[0] = new MyPropertyDescriptor("toto", attrArray, true);
            return new PropertyDescriptorCollection(pds);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
