#region Using directives

using System;
using System.Windows.Forms;
using VisualHint.SmartPropertyGrid;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Skybound.VisualTips.Rendering;

#endregion

namespace WindowsApplication
{
    public class MyPropertyGrid : VisualHint.SmartPropertyGrid.PropertyGrid
    {
        public class PlanetConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new object[] {
                    new Planet(1),
                    new Planet(2),
                    new Planet(3),
                    new Planet(4),
                    new Planet(5),
                    new Planet(6) }
                    );
            }
        }

        [TypeConverter(typeof(PlanetConverter))]
        public class Planet
        {
            private string _name = "";
            private int _position = -1;

            public Planet(string name)
            {
                _name = name;
            }

            public Planet(int position)
            {
                _position = position;
            }

            public override bool Equals(object obj)
            {
                return _name.Equals((obj as Planet)._name) && (_position == (obj as Planet)._position);
            }

            public override int GetHashCode()
            {
                return _name.GetHashCode() ^ _position.GetHashCode();
            }

            public override string ToString()
            {
                return _name;
            }
        }

        Skybound.VisualTips.VisualTipProvider vtp = new Skybound.VisualTips.VisualTipProvider();
        Skybound.VisualTips.VisualTip visualTip = new Skybound.VisualTips.VisualTip();

        private int _id = 1;

        private int upDownId1 = -1;
        private int upDownId2 = -1;
        private int upDownId3 = -1;
        private int upDownId4 = -1;
        private int upDownId5 = -1;
        private int buttonId1 = -1;
        private int buttonId2 = -1;
        private int buttonId3 = -1;
        private int listId1 = -1;
        private int listId2 = -1;
        private int listId3 = -1;
        private int sliderId1 = -1;
        private int sliderId2 = -1;
        private int uiTypeEditorId1 = -1;
        private int uiTypeEditorId2 = -1;
        private int fontId1 = -1;

        private TargetClass _targetInstance = new TargetClass();

		private ResourceManager resourceManager;

        public class MyBoolConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new object[] { MyBoolClass.False, MyBoolClass.True });
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    if (value.Equals("No value"))
                        return MyBoolClass.False;
                    else if (value.Equals("Value defined"))
                        return MyBoolClass.True;
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(String))
                {
                    if (value.Equals(MyBoolClass.True))
                        return "Value defined";
                    else if (value.Equals(MyBoolClass.False))
                        return "No value";
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        [TypeConverter(typeof(MyBoolConverter))]
        public class MyBoolClass
        {
            public static MyBoolClass True = new MyBoolClass(true);

            public static MyBoolClass False = new MyBoolClass(false);

            private bool _value;

            public MyBoolClass(bool value)
            {
                _value = value;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                if (obj is MyBoolClass)
                    return (this._value == (obj as MyBoolClass)._value);

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        protected override bool CanTakeTabKey(PropertyEnumerator propEnum, PropertyEnumerator startEnum)
        {
            Property property = propEnum.Property;

            if (!property.Enabled && property.CanBeDisabledManually)
                return true;

            return base.CanTakeTabKey(propEnum, startEnum);
        }

        public void Populate()
        {
            BeginUpdate();

            HideCategoriesMode = HideCategoriesModes.HideEmptyRootCategories;

            // Register event handlers
			PropertyCreated += new PropertyCreatedEventHandler(MyPropertyGrid_PropertyCreated);
			DisplayedValuesNeeded += new DisplayedValuesNeededEventHandler(MyPropertyGrid_DisplayedValuesNeeded);
			ValueValidation += new ValueValidationEventHandler(MyPropertyGrid_ValueValidation);
            PropertyUpDown += new PropertyUpDownEventHandler(MyPropertyGrid_PropertyUpDown);
            InPlaceCtrlVisible += new InPlaceCtrlVisibleEventHandler(MyPropertyGrid_InPlaceCtrlVisible);
            PropertyChanged += new PropertyChangedEventHandler(MyPropertyGrid_PropertyChanged);
            PropertyButtonClicked += new PropertyButtonClickedEventHandler(MyPropertyGrid_PropertyButtonClicked);
            PropertyEnabling += new PropertyEnablingEventHandler(MyPropertyGrid_PropertyEnabling);
            PropertyEnumerator propEnum;
            PropertyEnumerator subEnum;

            // All simple textboxes
            //---------------------

            PropertyEnumerator rootEnum = AppendRootCategory(_id++, "Textboxes alone");
            rootEnum.Property.Comment = "Icons can be set on categories and subcategories. They are supplied through an ImageList set on the grid.";
            rootEnum.Property.ImageIndex = 0;

            propEnum = AppendProperty(rootEnum, _id++, "Simple string", _targetInstance, "Editbox1", "");
            propEnum.Property.Value.Font = new Font(Font, FontStyle.Italic);
            propEnum.Property.Comment = "Icons can be set on property labels. They are supplied through an ImageList set on the grid.\n\nA bold font has been set on the Value. A validator is also attached: it checks that the first letter is uppercase.";
            propEnum.Property.ImageIndex = 1;
            propEnum.Property.Value.ImageSource = ImageSources.Grid;
            propEnum.Property.Value.ImageIndex = 5;

            // Masked edit
            propEnum = AppendProperty(rootEnum, _id++, "Masked textbox", _targetInstance, "Editbox11", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelMaskedEdit);
            propEnum.Property.Look = new PropertyMaskedEditLook("(000) 000-0000");
            propEnum.Property.Value.ForeColor = Color.Red;
            propEnum.Property.Value.Font = new Font(Font, FontStyle.Bold);
            propEnum.Property.Comment = "The text color can be changed on the value.";

            // Multiline editbox
            propEnum = AppendProperty(rootEnum, _id++, "Multiline Edit box", _targetInstance, "Editbox3", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelMultilineEdit);
            propEnum.Property.HeightMultiplier = 3;
            propEnum.Property.Look = new PropertyMultilineEditLook();
            propEnum.Property.Comment = "Icons can be set on values. They can be supplied through various sources. Here it comes from an ImageList.\n\nYou can choose the height of a multiline property.";
            propEnum.Property.Value.ImageSource = ImageSources.Grid;
            propEnum.Property.Value.ImageIndex = 3;

            // Add a simple edit box property attached to an integer
            propEnum = AppendProperty(rootEnum, _id++, "Simple integer", _targetInstance, "Editbox2", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEdit);
            propEnum.Property.Comment = "A validator object has been attached to this property to set a valid range [0..200].";

            // Password
            propEnum = AppendProperty(rootEnum, _id++, "Password", _targetInstance, "Editbox10", "");
            propEnum.Property.Comment = "This property uses custom look and feel classes to display a password.";
            
            // All buttons
            //------------

            rootEnum = AppendRootCategory(_id++, "Buttons");
            ((RootCategory)(rootEnum.Property)).ValueText = "(13 properties)";
            rootEnum.Property.Comment = "This category has a non editable value. This is useful for example to show a key child property value when the category is collapsed.";
            
            subEnum = AppendSubCategory(rootEnum, _id++, "Simple buttons");
            ((RootCategory)(subEnum.Property)).ValueText = "(3 properties)";
            subEnum.Property.Comment = "This subcategory has a non editable value. This is useful for example to show a key child property value when the category is collapsed.";

            buttonId1 = _id;
            propEnum = AppendManagedProperty(subEnum, _id++, "Button1", typeof(int), 100, "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditButton);
            propEnum.Property.Comment = "This button has a custom text.";

            buttonId2 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Button2", _targetInstance, "ButtonVar1", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelButton);
            propEnum.Property.Font = new Font(Font, FontStyle.Italic);
            propEnum.Property.Value.Font = new Font(Font, FontStyle.Bold);

            propEnum = AppendManagedProperty(subEnum, _id++, "Font", typeof(Font), new Font("Arial", 15f), "",
                new ShowChildPropertiesAttribute(false));

            // Checkboxes

            subEnum = AppendSubCategory(rootEnum, _id++, "Checkboxes");

            propEnum = AppendProperty(subEnum, _id++, "Enumeration", _targetInstance, "ButtonVar3", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Value.Font = new Font(Font, FontStyle.Bold);
            propEnum.Property.Comment = "This property is linked to an enumeration which has the [Flags] attribute.";

            buttonId3 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Boolean", _targetInstance, "ListVar3", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Comment = "Checkboxes can also be applied to a simple boolean and the displayed values can be customized.";

            propEnum = AppendManagedProperty(subEnum, _id++, "Nullable boolean", typeof(bool?), null, "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Value.ResetDisplayedValues(true, new object[] { true, "Yes", false, "No", null, "Undefined" });

            propEnum = AppendManagedProperty(subEnum, _id++, "Any class (2 values)", typeof(MyBoolClass), MyBoolClass.True, "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Comment = "The target property is a class whose TypeConverter publishes two values.";

            // Radio buttons

            subEnum = AppendSubCategory(rootEnum, _id++, "Radio buttons");

            propEnum = AppendProperty(subEnum, _id++, "Radio buttons", _targetInstance, "ButtonVar4", "");
            propEnum.Property.Look = new PropertyRadioButtonLook();
            propEnum.Property.Feel = GetRegisteredFeel(FeelRadioButton);
            propEnum.Property.Comment = "This property is linked to an enumeration and displays custom strings.";

            // UpDown buttons

            subEnum = AppendSubCategory(rootEnum, _id++, "Updown buttons");

            propEnum = AppendProperty(subEnum, _id++, "Time", _targetInstance, "Time", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelDateTime);
            //            propEnum.Property.Look = new PropertyDateTimeLook(DateTimePickerFormat.Time);
            propEnum.Property.Look = new PropertyDateTimeLook("mm:ss");

            upDownId1 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Enum", _targetInstance, "UpDownVar1", "");
            propEnum.Property.Value.BackColor = Color.Cornsilk;
            propEnum.Property.Feel = GetRegisteredFeel(FeelUpDown);
            propEnum.Property.Comment = "An updown control is also neat with an enumeration. Realtime mode has been set to true so no cancellation possible with Escape.";

            upDownId2 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Enum", _targetInstance, "UpDownVar2", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditUpDown);
            propEnum.Property.Value.Font = new Font(Font, FontStyle.Bold);
            propEnum.Property.Comment = "An updown control is also neat with an enumeratiom. Here I added a textbox.";

            upDownId3 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Boolean", _targetInstance, "UpDownVar3", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditUpDown);
            propEnum.Property.BackColor = Color.WhiteSmoke;
            propEnum.Property.ForeColor = Color.CadetBlue;
            propEnum.Property.Value.BackColor = GridBackColor;
            propEnum.Property.Value.ForeColor = GridForeColor;
            propEnum.Property.Comment = "An updown control can also be used with a boolean. Maybe not very common but it shows you how value types and inplace controls can be associated in a flexible way.";

            upDownId4 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Custom increment", _targetInstance, "UpDownVar4", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditUpDown);
            propEnum.Property.Comment = "The content is modified dynamically via notification. Here the increment is changed to 0.05. Limits between -1.0 and 1.0.";
            propEnum.Property.Value.Validator = new PropertyValidatorMinMax(-1.0, 1.0);

            upDownId5 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Custom list", _targetInstance, "UpDownVar5", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditUpDown);
            propEnum.Property.Comment = "The content of this property is changed at runtime. This is in fact a simple string.";
            
            // Lists
            //------

            rootEnum = AppendRootCategory(_id++, "Lists");

            listId1 = _id;
            propEnum = AppendProperty(rootEnum, _id++, "Enum", _targetInstance, "ListVar1", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelList);
            SetManuallyDisabled(propEnum, true);
            EnableProperty(propEnum, false);
            propEnum.Property.Comment = "Displays a simple enumeration. It has no textbox. A checkbox allows the user to disable the property.";

//            return;

            listId2 = _id;
            propEnum = AppendProperty(rootEnum, _id++, "Enum", _targetInstance, "ListVar2", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditList);
            propEnum.Property.Comment = "Compared to the previous one I added a textbox.";

            propEnum = AppendProperty(rootEnum, _id++, "Enum with icons", _targetInstance, "ListVar8",
                "By just adding an ImageList to the Value, you can nicely enhance the user experience.");
            propEnum.Property.Feel = GetRegisteredFeel(FeelList);
            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add((Bitmap)resourceManager.GetObject("browser_firefox"));
            il.Images.Add((Bitmap)resourceManager.GetObject("browser_internetexplorer"));
            il.Images.Add((Bitmap)resourceManager.GetObject("browser_netscape"));
            il.Images.Add((Bitmap)resourceManager.GetObject("browser_opera"));
//            il.ImageSize = new Size(34, 34);
  //          propEnum.Property.Value.ImageSize = new Size(34, 34);
    //        propEnum.Property.HeightMultiplier = 2;
            propEnum.Property.Value.ImageList = il;

            propEnum = AppendProperty(rootEnum, _id++, "Boolean", _targetInstance, "ListVar3", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditList);
            propEnum.Property.Comment = "The list is linked to a boolean but the displayed strings have been customized.";

            propEnum = AppendProperty(rootEnum, _id++, "Alpha color", _targetInstance, "ListVar4", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelList);
            propEnum.Property.Look = new PropertyAlphaColorLook();
            propEnum.Property.Comment = "Uses a custom look and feel to edit color with alpha";

            listId3 = _id;
            propEnum = AppendProperty(rootEnum, _id++, "List of dynamic strings", _targetInstance, "ListVar6", "", new UseFeelCacheAttribute());
            propEnum.Property.Comment = "This property displays a simple string. Possible values are requested to the client app by callback.";
//            propEnum.Property.Value.UseAutoCompletion = false;

            propEnum = AppendManagedProperty(rootEnum, _id++, "List of Planets / no names", typeof(Planet),
                new Planet(5), "", new PropertyValueDisplayedAsAttribute(true, new object[] {
                    new Planet(1), "Mercury",
                    new Planet(2), "Venus",
                    new Planet(3), "Earth",
                    new Planet(4), "Mars",
                    new Planet(5), "Jupiter",
                    new Planet(6),  "Saturn" }));
            propEnum.Property.Comment = "This property displays a Planet instance. Displayed strings are setup with a PropertyValueDisplayedAs attribute filled with <key,value> pairs where the key is a planet and the value its string representation.";

            propEnum = AppendManagedProperty(rootEnum, _id++, "List of Planets", typeof(Planet),
                new Planet("Venus"), "",
                new PropertyValueDisplayedAsAttribute(false, new object[] {
                    new Planet("Mercury"),
                    new Planet("Venus"),
                    new Planet("Earth"),
                    new Planet("Mars"),
                    new Planet("Jupiter"),
                    new Planet("Saturn") }));
            propEnum.Property.Comment = "This property displays a Planet instance. Displayed strings are setup with a PropertyValueDisplayedAs attribute filled with instance of named planets.";

            propEnum = AppendProperty(rootEnum, _id++, "Frequency", _targetInstance, "ListVar7", "");
            propEnum.Property.AddValue(PropertyUnitLook.UnitValue, _targetInstance, "Unit", null);
            propEnum.Property.Look = new PropertyUnitLook();
            propEnum.Property.Feel = GetRegisteredFeel(FeelEditUnit);
            propEnum.Property.Comment = "This property displays 2 values in one row.";
            propEnum.Property.GetValue(PropertyUnitLook.UnitValue).SetValue(TargetClass.Units.Hz);

            propEnum = AppendProperty(rootEnum, _id++, "Date", _targetInstance, "Date", "");

            // UITypeEditors
            //--------------
            
            rootEnum = AppendRootCategory(_id++, "UITypeEditors");
            rootEnum.Property.Comment = "SPG.Net is now compatible with all Microsoft and custom UITypeEditors.";

            propEnum = AppendProperty(rootEnum, _id++, "Font name", _targetInstance, "FontName", "");
            propEnum.Property.Look = new PropertyFontNameLook();

            propEnum = AppendProperty(rootEnum, _id++, "Long string", _targetInstance, "LongString", "");

            uiTypeEditorId1 = _id;
            propEnum = AppendProperty(rootEnum, _id++, "Colors", _targetInstance, "ColorArray", "");
            //            propEnum = AppendManagedProperty(rootEnum, _id++, "Colors", typeof(Color[]), _targetInstance.ColorArray, "");
            ExpandProperty(propEnum, true);
            propEnum = AppendManagedProperty(rootEnum, _id++, "Shortcut", typeof(Keys), Keys.A, "");
            propEnum = AppendManagedProperty(rootEnum, _id++, "Dock", typeof(DockStyle), DockStyle.Fill, "");
            uiTypeEditorId2 = _id;
            propEnum = AppendManagedProperty(rootEnum, _id++, "Color", typeof(Color), Color.Red, "",
                new PropertyLookAttribute(typeof(PropertyColorLook)));
            propEnum.Property.Comment = "Replicates the usual way to edit a color.";
            propEnum = AppendManagedProperty(rootEnum, _id++, "Cursor", typeof(Cursor), Cursors.Default, "");
            propEnum = AppendManagedProperty(rootEnum, _id++, "Alignment", typeof(ContentAlignment), ContentAlignment.BottomLeft, "");
            propEnum = AppendManagedProperty(rootEnum, _id++, "Icon", typeof(Icon), SystemIcons.Error, "",
                new ShowChildPropertiesAttribute(false));

            // Misc inplace controls and looks
            //--------------------------------

            rootEnum = AppendRootCategory(_id++, "Miscellaneous");

            subEnum = AppendSubCategory(rootEnum, _id++, "Trackbars");

            // The same with a slider control
            sliderId1 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Trackbar", _targetInstance, "Editbox5", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelTrackbar);
            propEnum.Property.Comment = "Result is deferred until validation. The limits have been set to [0 ..200].";

            sliderId2 = _id;
            propEnum = AppendProperty(subEnum, _id++, "Trackbar + textbox", _targetInstance, "Editbox5", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelTrackbarEdit);
            propEnum.Property.Comment = "Result is applied in realtime and modifies another property. The limits have been set to [0 ..200].";

            propEnum = AppendProperty(subEnum, _id++, "PointF/generic trackbars", _targetInstance, "MyPoint", "");
            propEnum.Property.Comment = "Trackbars are attached to the child properties. A trackbar can be associated with integer, float, double and decimal properties.";
            ExpandProperty(propEnum, true);

            subEnum = AppendSubCategory(rootEnum, _id++, "Others");

            propEnum = AppendProperty(subEnum, _id++, "Progress bar", _targetInstance, "Editbox5", "");
            propEnum.Property.Look = new PropertyProgressBarLook(true, "{0}%");
            propEnum.Property.Comment = "The value of the progress bar depends on the above sliders. The text has been customized to show a percentage.";

            // The same but disabled
            propEnum = AppendProperty(subEnum, _id++, "Disabled item", _targetInstance, "Editbox6", "");
            EnableProperty(propEnum, false);
            propEnum.Property.Comment = "This property has been explicitely disabled even if the underlying C# property is not readonly.";

            // A true readonly property
            propEnum = AppendProperty(subEnum, _id++, "Readonly item", _targetInstance, "Editbox12", "");
            propEnum.Property.Comment = "This underlying C# property is decorated with the Readonly attribute so you can't change this state. The color of the text can still be changed.";
            //            propEnum.Property.DisabledForeColor = Color.DarkKhaki;

            propEnum = AppendHyperLinkProperty(subEnum, _id++, "Disable left grid", "");
            propEnum.Property.Comment = "Do whatever you want from an hyperlink click event handler.";

            // Create the first category and add a comment
            //--------------------------------------------
            
            rootEnum = AppendRootCategory(_id++, "Reflection and customization");

            fontId1 = _id;
            propEnum = AppendProperty(rootEnum, _id++, "Font", _targetInstance, "MyFont", "",
                new PropertyHideAttribute("GdiCharSet"),
                new PropertyHideAttribute("GdiVerticalFont"),
                new PropertyHideAttribute("Unit"));
            propEnum.Property.Comment = "This property content is discovered by reflection but I customized it to hide certain subproperties. A color has also been added and a specific feel applied to the Size property.";
            propEnum.Property.Value.Validator = new PropertyValidatorFontSize(5.0f, 15.0f);

            propEnum = AppendProperty(rootEnum, _id++, "Rectangle", _targetInstance, "Rect", "");
            propEnum.Property.Feel = GetRegisteredFeel(FeelEdit);
            propEnum.Property.Comment = "This property content is discovered by reflection but I customized it to hide certain subproperties.";

            propEnum = AppendProperty(rootEnum, _id++, "Pen", _targetInstance, "Pen", "");
            propEnum.Property.Feel = null;
            propEnum.Property.Comment = "This property content is discovered by reflection but I customized it to hide certain subproperties. The parent node is custom drawn to reflect the color, width and type of the pen. The color has custom feel and look.";

            // Grouped property
            //-----------------

            rootEnum = AppendRootCategory(_id++, "Independent group");

            PropertyEnumerator grPropEnum = AppendManagedProperty(rootEnum, _id++, "Task name", typeof(String), "Write next version", "");

            PropertyEnumerator propEnum2 = AppendManagedProperty(grPropEnum, _id++, "Start date", typeof(DateTime), new DateTime(2008, 1, 1), "");
//            (propEnum2.Property.Look as PropertyDateTimeLook).Format = DateTimePickerFormat.Short;

            propEnum2 = AppendManagedProperty(grPropEnum, _id++, "End date", typeof(DateTime), new DateTime(2008, 3, 1), "");
//            (propEnum2.Property.Look as PropertyDateTimeLook).Format = DateTimePickerFormat.Short;

            ExpandProperty(grPropEnum, true);
            /*
            int labelColumnWidth;
            Size size = GetAdjustedRectangle(out labelColumnWidth);
            CommentsVisibility = false;
            ToolbarVisibility = false;
            Size = size;
            LabelColumnWidthRatio = (double)labelColumnWidth / (InternalGrid.ClientRectangle.Width - LeftColumnWidth);*/

            EndUpdate();
        }

        void MyPropertyGrid_PropertyEnabling(object sender, PropertyEnablingEventArgs e)
        {
        }
/*
        private Size GetAdjustedRectangle(out int labelColumnWidth)
        {
            int maxLabelWidth = 0;
            int maxValueWidth = 0;
            Size gridSize = Size.Empty;

            using (Graphics graphics = CreateGraphics())
            {
                PropertyEnumerator propEnum = FirstProperty;
                while (propEnum != propEnum.RightBound)
                {
                    Property property = propEnum.Property;

                    if (((property is RootCategory == false) || DrawManager.ShowCategoryVerticalLine) &&
                        (property is PropertyHyperLink == false))
                    {
                        Size size = Win32Calls.GetTextExtent(graphics, property.DisplayName, Font);

                        Rectangle labelRect = property.GetLabelTextRect(GetItemRect(propEnum), propEnum);
                        int width = labelRect.Left + size.Width;

                        if (width > maxLabelWidth)
                            maxLabelWidth = width;
                    }

                    PropertyValue propValue = property.Value;
                    if (propValue != null)
                    {
                        int width = Win32Calls.GetTextExtent(graphics, propValue.DisplayString, propValue.Font).Width;
                        if (width > maxValueWidth)
                            maxValueWidth = width;
                    }

                    gridSize.Height += property.HeightMultiplier * BasicPropertyHeight;

                    propEnum.MoveNext();
                }
            }

            labelColumnWidth = maxLabelWidth - LeftColumnWidth + GlobalTextMargin;
            gridSize.Width = maxLabelWidth + maxValueWidth + 3 * GlobalTextMargin + 2;
            gridSize.Height += 2; // border

            return gridSize;
        }
        */
        private void MyPropertyGrid_ValueValidation(object sender, ValueValidationEventArgs e)
        {
            if (e.ValueValidationResult == PropertyValue.ValueValidationResult.ExceptionByUITypeEditor)
            {
                MessageBox.Show(e.Exception.InnerException != null ? e.Exception.InnerException.Message : e.Message,
                    "Properties window", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return;
            }

            if (((e.ValueValidationResult & PropertyValue.ValueValidationResult.ErrorCode) != 0) &&
                (ValueNotValidBehaviorMode == ValueNotValidBehaviorModes.KeepFocus))
            {
                if (!vtp.IsTipDisplayed)
                {
                    Skybound.VisualTips.VisualTipDisplayOptions options =
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyDown |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyPress |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnLostFocus |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnMouseDown |
                        Skybound.VisualTips.VisualTipDisplayOptions.ForwardEscapeKey |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnTextChanged;

                    Control tb = (InPlaceControl as IInPlaceControl).TextBox;
                    if (tb == null)
                        tb = InPlaceControl as TextBoxBase;
                    if (tb == null)
                    {
                        tb = InPlaceControl;
                        options &= ~Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyDown &
                            ~Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyPress &
                            ~Skybound.VisualTips.VisualTipDisplayOptions.HideOnMouseDown &
                            ~Skybound.VisualTips.VisualTipDisplayOptions.HideOnTextChanged;
                    }

                    if (tb != null)
                    {
                        Rectangle rect = Rectangle.Intersect(InPlaceControl.RectangleToScreen(InPlaceControl.ClientRectangle), InternalGrid.RectangleToScreen(InternalGrid.ClientRectangle));
                        visualTip.Text = e.Message;
                        visualTip.FooterText = "Need help? Press F1";
                        visualTip.Image = (Bitmap)resourceManager.GetObject("warning");
                        visualTip.Font = new Font(visualTip.Font, FontStyle.Regular);
                        visualTip.Title = e.PropertyEnum.Property.DisplayName;
                        vtp.SetVisualTip(e.PropertyEnum, visualTip);
                        vtp.ShowTip(visualTip, rect, tb, options);
                    }
                }
            }
            else
                vtp.HideTip();
        }

        void MyPropertyGrid_PropertyButtonClicked(object sender, PropertyButtonClickedEventArgs e)
        {
            MessageBox.Show("OnPropertyButtonClicked");

            e.PropertyEnum.Property.Value.SetValue(99);

            e.PropertyChanged = true;
        }

        void MyPropertyGrid_PropertyChanged(object sender, VisualHint.SmartPropertyGrid.PropertyChangedEventArgs e)
        {
            if (e.PropertyEnum.Property.Id == uiTypeEditorId1)
            {
                PropertyEnumerator childEnum = e.PropertyEnum.Children;
                childEnum.MoveFirst();
                int i = 1;
                while (childEnum != childEnum.RightBound)
                {
                    childEnum.Property.DisplayName = "Color " + (i++);
                    childEnum.MoveNext();
                }
            }
        }

        void MyPropertyGrid_InPlaceCtrlVisible(object sender, InPlaceCtrlVisibleEventArgs e)
        {
            if (SelectedObjects.Length > 0)
                return;

            if (e.PropertyEnum.Property.Id == upDownId1)
            {
                if (e.InPlaceCtrl is PropInPlaceUpDown) ((PropInPlaceUpDown)e.InPlaceCtrl).RealtimeChange = true;
            }
            else if (e.PropertyEnum.Property.Id == upDownId2)
            {
                if (e.InPlaceCtrl is PropInPlaceUpDown) ((PropInPlaceUpDown)e.InPlaceCtrl).RealtimeChange = false;
            }
            else if (e.PropertyEnum.Property.Id == upDownId3)
            {
                if (e.InPlaceCtrl is PropInPlaceUpDown) ((PropInPlaceUpDown)e.InPlaceCtrl).RealtimeChange = true;
            }
            else if (e.PropertyEnum.Property.Id == upDownId4)
            {
                if (e.InPlaceCtrl is PropInPlaceUpDown) ((PropInPlaceUpDown)e.InPlaceCtrl).RealtimeChange = false;
            }
            else if (e.PropertyEnum.Property.Id == upDownId5)
            {
                if (e.InPlaceCtrl is PropInPlaceUpDown) ((PropInPlaceUpDown)e.InPlaceCtrl).RealtimeChange = true;
            }
            else if (e.PropertyEnum.Property.Id == sliderId2)
            {
                if (e.InPlaceCtrl is PropInPlaceTrackbar) ((PropInPlaceTrackbar)e.InPlaceCtrl).RealtimeChange = true;
            }
            else if (e.InPlaceCtrl is PropInPlaceTrackbar)
            {
                ((PropInPlaceTrackbar)e.InPlaceCtrl).RealtimeChange = false;
            }
            else if (e.InPlaceCtrl is PropInPlaceButton)
            {
                if (e.PropertyEnum.Property.Id == buttonId1)
                    (e.InPlaceCtrl as PropInPlaceButton).ButtonText = "test";
                else
                    (e.InPlaceCtrl as PropInPlaceButton).ButtonText = "...";
            }

            // If you want instant validation while typing in a textbox, uncomment this:
            // But beware, in this case, the options for vtp.ShowTip() must not include HideOnLostFocus,
            // HideOnMouseDown and ForwardEscapeKey
/*
            Control tb = (InPlaceControl as IInPlaceControl).TextBox;
            if (tb == null)
                tb = InPlaceControl as TextBoxBase;
            if (tb != null)
                tb.TextChanged += new EventHandler(TextBox_TextChanged);
 */
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            Control tb = (InPlaceControl as IInPlaceControl).TextBox;
            if (tb == null)
                tb = InPlaceControl as TextBoxBase;

            if (tb != null)
            {
                PropertyValue propValue = SelectedPropertyEnumerator.Property.Value;
                object value = null;

                vtp.HideTip();

                try
                {
                    value = propValue.ConvertDisplayedStringToValue(tb.Text);
                }
                catch (Exception ex)
                {
                    NotifyValueValidation(new ValueValidationEventArgs(SelectedPropertyEnumerator, SelectedPropertyEnumerator, value,
                        PropertyValue.ValueValidationResult.TypeConverterFailed, ex));
                    return;
                }

                PropertyEnumerator invalidPropEnum;
                if (!propValue.ValidateValue(value, out invalidPropEnum))
                {
                    NotifyValueValidation(new ValueValidationEventArgs(SelectedPropertyEnumerator, invalidPropEnum, value, PropertyValue.ValueValidationResult.ValidatorFailed));
                }
            }
        }

        void MyPropertyGrid_PropertyCreated(object sender, PropertyCreatedEventArgs e)
        {
            if ((e.PropertyEnum.Property.Id == fontId1) && (_targetInstance.MyFont != null))
            {
                PropertyEnumerator childEnum = e.PropertyEnum.Children;
                if (childEnum.Count > 0)
                {
                    childEnum.MoveFirst();
                    childEnum.Property.Comment = "Uses a UITypeEditor. By the way, you can set comments to auto-discovered subproperties...";
                    childEnum.MoveNext();
                    childEnum.Property.Comment = "A custom feel has been applied to this property. It is done by setting an attribute to its parent. Also a validator constrains the value between 5 and 15.";
                    childEnum.Property.Value.Validator = new PropertyValidatorMinMax((Single)5.0, (Single)15.0);
                    childEnum.MoveNext();

                    PropertyEnumerator propEnum = InsertProperty(childEnum, 150, "Color", _targetInstance, "FontColor", "");
                    propEnum.Property.Feel = GetRegisteredFeel(FeelList);
                    propEnum.Property.Look = new PropertyAlphaColorLook();
                    propEnum.Property.Comment = "This color has been added to the children of the font. It uses an alpha color editor.";

                    propEnum.MoveNext();
                    propEnum.Property.Comment = "The text of a checkbox can be removed.";
                    propEnum.MoveNext();
                    propEnum.Property.Comment = "The text of a checkbox can be customized.";
                }
            }
            else if (e.PropertyEnum.Property.Id == uiTypeEditorId1)
            {
                PropertyEnumerator childEnum = e.PropertyEnum.Children;
                childEnum.MoveFirst();
                int i = 1;
                while (childEnum != childEnum.RightBound)
                {
                    childEnum.Property.DisplayName = "Color " + (i++);
                    childEnum.MoveNext();
                }
            }
        }

        void MyPropertyGrid_DisplayedValuesNeeded(object sender, DisplayedValuesNeededEventArgs e)
        {
            if (e.PropertyEnum.Property.Id == listId3)
                e.DisplayedValues = new string[] { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn" };
            else if (e.PropertyEnum.Property.Id == buttonId3)
                e.DisplayedValues = new string[] { "Enabled", "Disabled" };
        }

        void MyPropertyGrid_PropertyUpDown(object sender, PropertyUpDownEventArgs e)
        {
            if (e.PropertyEnum.Property.Id == upDownId4)
            {
                try
                {
                    e.Value = (Double.Parse(e.Value) + (e.ButtonPressed == PropertyUpDownEventArgs.UpDownButtons.Up ? 0.05 : -0.05)).ToString();
                }
                catch (FormatException)
                {
                }
            }
            else if (e.PropertyEnum.Property.Id == upDownId5)
            {
                int index = _targetInstance.upDownString.IndexOf(e.Value);
                try
                {
                    e.Value = (string)_targetInstance.upDownString[index + (e.ButtonPressed == PropertyUpDownEventArgs.UpDownButtons.Up ? -1 : 1)];
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        public MyPropertyGrid()
        {
            ReadOnlyVisual = ReadOnlyVisuals.ReadOnlyFeel;

            resourceManager = new ResourceManager("WindowsApplication.MainResources", Assembly.GetExecutingAssembly());

            ImageList = new ImageList();
//            ImageList.TransparentColor = Color.FromArgb(255, 0, 255);
  //          ImageList.ImageSize = new Size(16,15);
    //        ImageList.Images.AddStrip((Bitmap)resourceManager.GetObject("icons"));
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_1_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_2_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_3_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_4_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_5_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_6_16"));
            ImageList.Images.Add((Bitmap)resourceManager.GetObject("_7_16"));

            DefaultFeel = GetRegisteredFeel(FeelEdit); 
            
            DrawingManager = DrawManagers.DotnetDrawManager;

            Skybound.VisualTips.Rendering.VisualTipOfficeRenderer renderer = new Skybound.VisualTips.Rendering.VisualTipOfficeRenderer();
            renderer.Preset = VisualTipOfficePreset.Hazel;
            renderer.BackgroundEffect = Skybound.VisualTips.Rendering.VisualTipOfficeBackgroundEffect.Gradient;
            vtp.Renderer = renderer;
            vtp.DisplayMode = Skybound.VisualTips.VisualTipDisplayMode.Manual;
            vtp.Opacity = 1.0;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if ((m.Msg == Win32Calls.WM_KEYDOWN) && (m.HWnd == InternalGrid.Handle))
            {
                if ((Keys)(int)m.WParam == Keys.Delete)
                {
//                    DeleteProperty(SelectedPropertyEnumerator);
  //                  return true;
                }
            }

            return base.ProcessKeyPreview(ref m);
        }

        private object propertyStates = null;

        protected override void OnDrawingManagerChanged(EventArgs e)
        {
            if (DrawingManager == DrawManagers.LightColorDrawManager)
            {
                LightColorDrawManager manager = DrawManager as LightColorDrawManager;
                manager.SetCategoryBackgroundColors(Color.FromArgb(199,218,255), Color.White);
                manager.SetSubCategoryBackgroundColors(Color.FromArgb(197,242,191), Color.White);
            }

            if (DrawingManager == DrawManagers.CustomDrawManager)
            {
                propertyStates = SavePropertiesStates(PropertyStateFlags.All);

                GridBackColor = Color.Black;
                GridForeColor = Color.FromArgb(192, 192, 192);
                GridColor = Color.FromArgb(43, 43, 64);

                PropertyEnumerator propEnum = FirstProperty;
                while (propEnum != propEnum.RightBound)
                {
                    if (propEnum.Property.IsSetBackColor)
                        propEnum.Property.BackColor = Color.Empty;
                    if (propEnum.Property.IsSetForeColor)
                        propEnum.Property.ForeColor = Color.Empty;

                    PropertyValue propValue = propEnum.Property.Value;
                    if (propValue != null)
                    {
                        if (propEnum.Property.Value.IsSetBackColor)
                            propEnum.Property.Value.BackColor = Color.Empty;
                        if (propEnum.Property.Value.IsSetForeColor)
                            propEnum.Property.Value.ForeColor = Color.Empty;
                    }

                    propEnum.MoveNext();
                }

                LightColorDrawManager manager = DrawManager as LightColorDrawManager;
                if (manager != null)
                {
                    manager.SetCategoryBackgroundColors(Color.FromArgb(65, 76, 96), Color.FromArgb(65, 76, 96));
                    manager.SetSubCategoryBackgroundColors(Color.FromArgb(49, 49, 57), Color.FromArgb(49, 49, 57));
                    manager.UseBoldFontForCategories = true;
                }
            }
            else
            {
                GridBackColor = SystemColors.Window;
                GridForeColor = SystemColors.ControlText;
                GridColor = SystemColors.ActiveBorder;

                if (propertyStates != null)
                {
                    RestorePropertiesStates(propertyStates);
                    propertyStates = null;
                }
            }
            
            base.OnDrawingManagerChanged(e);
		}
        
        private Point _mouseStart;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SPGMouseEventArgs args = (e as SPGMouseEventArgs);
                if (args != null)
                {
                    if ((args.HitTest == HitTests.LeftColumn) || (args.HitTest == HitTests.Label))
                    {
                        _mouseStart = new Point(args.X, args.Y);
                        //                    LeftColumnWidth = LeftColumnWidth * 2;
//                        Cursor = Cursors.Hand;
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((_mouseStart != Point.Empty) && (e.Button == MouseButtons.Left))
            {
//                Cursor = Cursors.Hand;
                int offset = (e.Y - _mouseStart.Y) / BasicPropertyHeight;
                if (offset < 0)
                {
                    for(int i=0;i<-offset;i++)
                        Win32Calls.SendMessage(InternalGrid.Handle, Win32Calls.WM_VSCROLL, (IntPtr)Win32Calls.MakeLong(Win32Calls.SB_LINEDOWN, 0), IntPtr.Zero);

                    _mouseStart.Y += offset * BasicPropertyHeight;
                }
                else if (offset > 0)
                {
                    for (int i = 0; i < offset; i++)
                        Win32Calls.SendMessage(InternalGrid.Handle, Win32Calls.WM_VSCROLL, (IntPtr)Win32Calls.MakeLong(Win32Calls.SB_LINEUP, 0), IntPtr.Zero);

                    _mouseStart.Y += offset * BasicPropertyHeight;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_mouseStart != Point.Empty)
            {
//                LeftColumnWidth = LeftColumnWidth / 2;
                _mouseStart = Point.Empty;
                Cursor = Cursors.Default;
            }

            base.OnMouseUp(e);
        }

        private bool IsScrollbarVisible()
        {
            return ((Win32Calls.GetWindowLong32(InternalGrid.Handle, Win32Calls.GWL_STYLE) & Win32Calls.WS_VSCROLL) != 0);
        }
    }
}
