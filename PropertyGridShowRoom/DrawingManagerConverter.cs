using System;
using System.ComponentModel;

namespace VisualHint.SmartPropertyGrid
{
    public class DrawingManagerConverter : EnumConverter
    {
        protected class DrawingManagerPropertyDescriptor : SimplePropertyDescriptor
        {
            private CustomDrawManager _drawManager;
            private PropertyDescriptor _pd;

            public DrawingManagerPropertyDescriptor(CustomDrawManager drawManager, PropertyDescriptor pd) :
                base(typeof(CustomDrawManager), pd.Name, pd.PropertyType)
            {
                _drawManager = drawManager;
                _pd = pd;
            }

            public override object GetValue(object component)
            {
                return _pd.GetValue(_drawManager);
            }

            public override void SetValue(object component, object value)
            {
                _pd.SetValue(_drawManager, value);
            }

            public override bool IsReadOnly
            {
                get
                {
                    return _pd.IsReadOnly;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return _pd.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return _pd.Description;
                }
            }

            public override string Category
            {
                get
                {
                    return _pd.Category;
                }
            }

            public override bool IsBrowsable
            {
                get
                {
                    return _pd.IsBrowsable;
                }
            }
        }

        public DrawingManagerConverter()
            : base(typeof(PropertyGrid.DrawManagers))
        {
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            PropertyGrid grid = (PropertyGrid)context.Instance;
            return (grid.DrawManager != null);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyGrid grid = (PropertyGrid)context.Instance;
            PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(grid.DrawManager, new Attribute[] { new BrowsableAttribute(true) });
            DrawingManagerPropertyDescriptor[] pds = new DrawingManagerPropertyDescriptor[coll.Count];
            int index = 0;
            foreach (PropertyDescriptor pd in coll)
            {
                pds[index++] = new DrawingManagerPropertyDescriptor(grid.DrawManager, pd);
            }

            return new PropertyDescriptorCollection(pds);
        }
    }
}
