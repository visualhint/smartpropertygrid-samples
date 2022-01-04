using System;
using VisualHint.SmartPropertyGrid;
using System.ComponentModel;
using Skybound.VisualTips.Rendering;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WindowsApplication
{
    public class MetaPropertyGrid : PropertyGrid
    {
        private static string _helpText = "Press F1 for some help.";

        Skybound.VisualTips.VisualTipProvider _helpVtp = new Skybound.VisualTips.VisualTipProvider();
        Skybound.VisualTips.VisualTip _helpVisualTip = new Skybound.VisualTips.VisualTip();
        Skybound.VisualTips.VisualTipProvider _errorVtp = new Skybound.VisualTips.VisualTipProvider();
        Skybound.VisualTips.VisualTip _errorVisualTip = new Skybound.VisualTips.VisualTip();

        PropertyGrid _targetPropertyGrid;

		private ResourceManager resourceManager;

        int _id = 1;

        private int _labelColumnWidthRatio;
        public int LabelColumnWidthRatio2
        {
            set
            {
                _labelColumnWidthRatio = value;
                if (_targetPropertyGrid != null)
                    _targetPropertyGrid.LabelColumnWidthRatio = (double)_labelColumnWidthRatio/100.0;
            }
            get { return _labelColumnWidthRatio; }
        }

        private bool _rightToLeft;

        public bool RightToLeft2
        {
            get { return _rightToLeft; }
            set
            {
                _rightToLeft = value;
                if (_targetPropertyGrid != null)
                {
                    if (_rightToLeft)
                        _targetPropertyGrid.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    else
                        _targetPropertyGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
                }
            }
        }

        public MetaPropertyGrid()
        {
            resourceManager = new ResourceManager("WindowsApplication.MainResources", Assembly.GetExecutingAssembly());

            RegisterDropDownContent(typeof(Color), typeof(AlphaColorPicker));

//            _helpVisualTip.Font = new Font(_helpVisualTip.Font, FontStyle.Regular);

            Skybound.VisualTips.Rendering.VisualTipOfficeRenderer renderer = new Skybound.VisualTips.Rendering.VisualTipOfficeRenderer();
            renderer.Preset = VisualTipOfficePreset.DeepBlue;
            renderer.BackgroundEffect = Skybound.VisualTips.Rendering.VisualTipOfficeBackgroundEffect.Glass;
            _helpVtp.Renderer = renderer;
            _helpVtp.DisplayMode = Skybound.VisualTips.VisualTipDisplayMode.HelpRequested;
            _helpVtp.Opacity = 1.0;

            _helpVisualTip.TitleImage = (Bitmap)resourceManager.GetObject("info");
            _errorVisualTip.TitleImage = (Bitmap)resourceManager.GetObject("warning_small");

            renderer = new Skybound.VisualTips.Rendering.VisualTipOfficeRenderer();
            renderer.Preset = VisualTipOfficePreset.Brown;
            renderer.BackgroundEffect = Skybound.VisualTips.Rendering.VisualTipOfficeBackgroundEffect.Gradient;
            _errorVtp.Renderer = renderer;
            _errorVtp.DisplayMode = Skybound.VisualTips.VisualTipDisplayMode.Manual;
//            _errorVtp.Opacity = 1.0;

            double H, S, L;
            ColorUtils.RGBtoHSL(GridColor, out H, out S, out L);
            PropertyLabelBackColor = ColorUtils.HSLtoRGB(H, S, L + 2 * (1 - L) / 3.0);
            ColorUtils.RGBtoHSL(PropertyLabelBackColor, out H, out S, out L);
            _hoveredPropertyBackColor = ColorUtils.HSLtoRGB(H, S, 9 * L / 10.0);
        }

        private Color _hoveredPropertyBackColor;

        private int _useCustomDrawingManagerId;
        private int _drawingManagerId;
        private int _labelColumnWidthRatioId;
        private int _enabledId;
        private int _disableModeId;
        private int _disableModeGrayedOutId;
        private int _navigationKeyModeId;
        private int _tabKeyNavigationMode;
        private int _selectedPropertyId;
        private int _propertyValueId;
        private int _fontId;
        private int _propertyFontId = -1;
        private int _propertyValueFontId = -1;
        private PropertyEnumerator _commentsHeightPropEnum;
        private int _propertyEnabledId;
        private int _propertyExpandedId;
        private int _propertyManuallyDisabledId;
        private int _useCustomColorsId;

        public void Populate(PropertyGrid targetPropertyGrid)
        {
            BeginUpdate();

            _targetPropertyGrid = targetPropertyGrid;
            _rightToLeft = (targetPropertyGrid.RightToLeft == System.Windows.Forms.RightToLeft.Yes);
            _targetPropertyGrid.PropertySelected += new PropertySelectedEventHandler(OnTargetPropertySelected);
            _targetPropertyGrid.SizeChanged += new EventHandler(OnTargetGridSizeChanged);
            _targetPropertyGrid.RightToLeftChanged += new EventHandler(_targetPropertyGrid_RightToLeftChanged);
            _targetPropertyGrid.DrawingManagerChanged += new DrawingManagerChangedEventHandler(_targetPropertyGrid_DrawingManagerChanged);

            _labelColumnWidthRatio = (int)Math.Round(_targetPropertyGrid.LabelColumnWidthRatio * 100.0);

            PropertyEnumerator rootEnum;
            PropertyEnumerator subcatEnum;
            PropertyEnumerator propEnum;

            rootEnum = AppendRootCategory(_id++, "Appearance");

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Global");

            _drawingManagerId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "DrawingManager", _targetPropertyGrid, "DrawingManager", _helpText,
                new TypeConverterAttribute(typeof(DrawingManagerConverter)));
            propEnum.Property.Tag = "A drawing manager is a class that contains all the drawing code necessary to "+
                "give a unified and custom appearance to the whole PropertyGrid. Three built-in managers are provided. "+
                "You can define your own ones by deriving from a base or existing class.\n\nIn this sample, the colors "+
                "of the LightColorManager (and of the grid) are set in the OnDrawingManagerChanged notification method.";
            SelectProperty(propEnum);

            _useCustomDrawingManagerId = _id;
            propEnum = AppendManagedProperty(subcatEnum, _id++, "Use a custom drawing manager", typeof(bool),
                false, _helpText, new PropertyValueDisplayedAsAttribute(new string[] { "", "" }),
                new RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties.All));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "When set to true, this will set a custom drawing manager, derived from the "+
                "LightColorManager, and custom colors will be set to the grid.";

            _fontId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "Font", _targetPropertyGrid, "Font", _helpText,
                new PropertyHideAttribute("GdiCharSet"),
                new PropertyHideAttribute("GdiVerticalFont"),
                new PropertyHideAttribute("Unit"),
                new PropertyHideAttribute("Strikeout"),
                new PropertyHideAttribute("Underline"),
                new PropertyFeelAttribute("Size", PropertyGrid.FeelEditUpDown),
                new PropertyLookAttribute("Italic", typeof(PropertyCheckboxLook)),
                new PropertyLookAttribute("Bold", typeof(PropertyCheckboxLook)),
                new PropertyFeelAttribute("Italic", VisualHint.SmartPropertyGrid.PropertyGrid.FeelCheckbox),
                new PropertyFeelAttribute("Bold", VisualHint.SmartPropertyGrid.PropertyGrid.FeelCheckbox),
                new PropertyValueDisplayedAsAttribute("Italic", new string[2] { "Yes", "No" }),
                new PropertyValueDisplayedAsAttribute("Bold", new string[2] { "Yes", "No" }));
            propEnum.Property.Tag = "This is the global font of the grid. Expand this node and notice how some "+
                "properties of the font have been removed (GdiCharSet, Unit, ...).\n\nThis global font can be "+
                "overriden per property. For example, the property with the label \"Masked textbox\" will keep "+
                "its bold/red setting in the value column.";

            propEnum = AppendProperty(subcatEnum, _id++, "Show default values", this, "ShowDefaultValues", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "Exceptionnaly we will act on this PropertyGrid here because it features "+
                "more DefaultValue attributes than the right grid. When set to true, values are displayed in bold "+
                "when different from a default value specified with an attribute or when a ShouldSerialize method "+
                "says so. This is to replicate a feature of the Microsoft PropertyGrid.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Comments");
            subcatEnum.Property.Comment = _helpText;
            subcatEnum.Property.Tag = "Did you see the checkbox at the left? Nice trick to save one line in "+
                "the grid isn't it? Look also how it controls the Enabled state of all the hierarchy.";
            SetManuallyDisabled(subcatEnum, _targetPropertyGrid, "CommentsVisibility");

            propEnum = AppendProperty(subcatEnum, _id++, "CommentsBackColor", _targetPropertyGrid, "CommentsBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "CommentsForeColor", _targetPropertyGrid, "CommentsForeColor", "");
            propEnum.Property.Look = new PropertyColorLook();

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Toolbar");

            propEnum = AppendProperty(subcatEnum, _id++, "ToolbarVisibility", _targetPropertyGrid, "ToolbarVisibility", "",
                new PropertyValueDisplayedAsAttribute(new string[] { "Visible", "Invisible" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();

            propEnum = AppendProperty(subcatEnum, _id++, "DisplayMode", _targetPropertyGrid, "DisplayMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelRadioButton);
            propEnum.Property.Look = new PropertyRadioButtonLook();
            propEnum.Property.Tag = "The Categorized and FlatSorted modes are the ones you know in the Microsoft "+
                "PropertyGrid. Additionally, the Flat mode is a linear view of the properties in the raw order they "+
                "were created.\n\nAt last, the PropertyComparer property (not shown here) allows you to set a "+
                "custom sort algorythm.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Main colors");
            subcatEnum.Property.Comment = _helpText;
            subcatEnum.Property.Tag = "These are the colors you will modify to accomodate the look of your application.";

            _useCustomColorsId = _id;
            propEnum = AppendManagedProperty(subcatEnum, _id++, "Use custom colors", typeof(bool), false, _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "Change the checkbox value and look how easy it is to change look and feel classes "+
                "assigned to the next properties.";

            propEnum = AppendProperty(subcatEnum, _id++, "GridBackColor", _targetPropertyGrid, "GridBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "GridColor", _targetPropertyGrid, "GridColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "GridForeColor", _targetPropertyGrid, "GridForeColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "HighlightedTextColor", _targetPropertyGrid, "HighlightedTextColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "SelectedBackColor", _targetPropertyGrid, "SelectedBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "SelectedNotFocusedBackColor", _targetPropertyGrid, "SelectedNotFocusedBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "DisabledForeColor", _targetPropertyGrid, "DisabledForeColor", _helpText);
            propEnum.Property.Look = new PropertyColorLook();
            propEnum.Property.Tag = "Accessibility is improved by letting you chose the color of disabled properties.";
            propEnum = AppendProperty(subcatEnum, _id++, "ReadOnlyForeColor", _targetPropertyGrid, "ReadOnlyForeColor", _helpText);
            propEnum.Property.Look = new PropertyColorLook();
            propEnum.Property.Tag = "Accessibility is improved by letting you chose the color of readonly properties.";

            propEnum = AppendProperty(subcatEnum, _id++, "PropertyLabelBackColor", _targetPropertyGrid, "PropertyLabelBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(subcatEnum, _id++, "PropertyValueBackColor", _targetPropertyGrid, "PropertyValueBackColor", "");
            propEnum.Property.Look = new PropertyColorLook();

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Layout");

            propEnum = AppendProperty(subcatEnum, _id++, "Right To Left", this, "RightToLeft2", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" })); 
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "SPG is the only PropertyGrid to support RightToLeft. You can try it right now but this is even better with arabic or hebrew languages.";

            _labelColumnWidthRatioId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "LabelColumnWidthRatio", this, "LabelColumnWidthRatio2", _helpText);
            propEnum.Property.Value.Validator = new PropertyValidatorMinMax(0, 100);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelTrackbarEdit);
            propEnum.Property.Tag = "The trackbar will set the width of the labels column in realtime.\n\nA "+
                "validator has been assigned to ensure that a value between 0 and 100 would produce the expected "+
                "ratio between 0.0 and 1.0.";

            propEnum = AppendProperty(subcatEnum, _id++, "PropertyVerticalMargin", _targetPropertyGrid, "PropertyVerticalMargin", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelEditUpDown);
            propEnum.Property.Value.Validator = new PropertyValidatorMinMax(2, 6);
            propEnum.Property.Tag = "This property modifies the space between texts and horizontal lines to your taste. "+
                "Of course it takes its meaning in \"reasonable\" values. Too much space would not be visually pleasant. "+
                "That's why I have added in this sample a validator to restrict the values in a small interval.";

            propEnum = AppendProperty(subcatEnum, _id++, "CommentsHeight", _targetPropertyGrid, "CommentsHeight", _helpText);
            propEnum.Property.Value.Validator = new PropertyValidatorMinMax(40, InternalGrid.Height - 2 * BasicPropertyHeight);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelTrackbarEdit);
            _commentsHeightPropEnum = propEnum;
            propEnum.Property.Tag = "A MinMax validator is assigned to this property and the Max is modified in realtime "+
                "to accomodate the height of the target PropertyGrid. Make a try: modify the size of the form while the "+
                "trackbar is focused and see what happens...";

            propEnum = AppendProperty(subcatEnum, _id++, "MS Indentation", _targetPropertyGrid, "ShowAdditionalIndentation", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "In certain circumstances, showing the usual indentation of the Microsoft "+
                "PropertyGrid can be better. This is the case for example when you have a list of properties, "+
                "some having a checkbox at the left of the label and some having some children. The SPG way "+
                "would create a zigzag effect, although the MSPG would not.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Misc");

            propEnum = AppendProperty(subcatEnum, _id++, "EllipsisMode", _targetPropertyGrid, "EllipsisMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "SPG gives you the flexibility to choose where you want to display ellipsis on " +
                "strings that are right truncated.";

            propEnum = AppendProperty(subcatEnum, _id++, "ToolTipMode", _targetPropertyGrid, "ToolTipMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "SPG gives you the flexibility to choose where you want to display tooltips on " +
                "strings that are right truncated.";

            rootEnum = AppendRootCategory(_id++, "Behavior");

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Disabilities");

            propEnum = AppendProperty(subcatEnum, _id++, "CommentsLock", _targetPropertyGrid, "CommentsLock", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "Locked", "Unlocked" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "A simple way to prevent the user to resize the comments area.";

            propEnum = AppendProperty(subcatEnum, _id++, "ColumnsLock", _targetPropertyGrid, "ColumnsLock", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "Locked", "Unlocked" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "A simple way to prevent the user to resize the columns.";

            _enabledId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "Enabled", _targetPropertyGrid, "Enabled", _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "The Microsoft ProperyGrid offers a single disabled mode where the user " +
                "can do nothing on the control. SPG, thanks to the next property, offers various modes of " +
                "operation. So uncheck this property and play with the two next ones (they will be automatically " +
                "enabled.";

            _disableModeId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "DisableMode", _targetPropertyGrid, "DisableMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelRadioButton);
            propEnum.Property.Look = new PropertyRadioButtonLook();
            EnableProperty(propEnum, false);
            propEnum.Property.Tag = "When Enabled is set to false, you can vary the behavior of the grid with "+
                "this mode. The names are quite explicit so no need of further explanations here. Just note that "+
                "in Simple mode with DisableModeGrayedOut set to false (just below), all properties will be "+
                "displayed in \"readonly\" color. Change ReadOnlyForeColor above to standard text, grayed or "+
                "whatever color if you don't like it.";

            _disableModeGrayedOutId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "DisableModeGrayedOut", _targetPropertyGrid, "DisableModeGrayedOut", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            EnableProperty(propEnum, false);
            propEnum.Property.Tag = "When set to true, forces the color of any text to gray when the grid is "+
                "disabled. Since this is not the behavior that everyone wants, it can be unchecked.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Navigation");

            _navigationKeyModeId = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "NavigationKeyMode", _targetPropertyGrid, "NavigationKeyMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelRadioButton);
            propEnum.Property.Look = new PropertyRadioButtonLook();
            propEnum.Property.Tag = "This was so many time requested that VisualHint did it. With the TabKey "+
                "mode, every textbox can be visited with the single press of the TAB, Return and escape "+
                "keys.\n\nThe next property gives the choice of various non exclusive submodes.";

            _tabKeyNavigationMode = _id;
            propEnum = AppendProperty(subcatEnum, _id++, "TabKeyNavigationMode", _targetPropertyGrid, "TabKeyNavigationMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            EnableProperty(propEnum, false);
            propEnum.Property.Tag = "Mix these various submodes and play with them. You will like one "+
                "combination for your application, for sure.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Value validation");

            propEnum = AppendProperty(subcatEnum, _id++, "ValueNotValidBehaviorMode", _targetPropertyGrid, "ValueNotValidBehaviorMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelRadioButton);
            propEnum.Property.Look = new PropertyRadioButtonLook();
            propEnum.Property.Tag = "Introduced in version 2.2 the new KeepFocus mode lets you keep the focus "+
                "on an invalid value and display whatever clue you want (like an icon) to bring user's attention. An " +
                "invalid value is one that can't be converted back from a string to a given type by the "+
                "associated TypeConverter or that can't pass the test of an attached validator class. It can also "+
                "happen when your setter method raises an exception.\n\n"+
                "In this sample, a Skybound VisualTips© tip is shown in place.\n\nPlay in both " +
                "PropertyGrids to see where you can enter invalid values.";

            subcatEnum = AppendSubCategory(rootEnum, _id++, "Misc");

            propEnum = AppendProperty(subcatEnum, _id++, "ReadOnlyVisual", _targetPropertyGrid, "ReadOnlyVisual", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelRadioButton);
            propEnum.Property.Look = new PropertyRadioButtonLook();
            propEnum.Property.Tag = "Set how readonly properties are rendered. They can use readonly textboxes "+
                "so that it's possible to copy the text in the clipboard or they can be completely disabled.";

            propEnum = AppendProperty(subcatEnum, _id++, "ValueCyclingMode", _targetPropertyGrid, "DoubleClickCycleMode", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "Clicking on a label or value will normally set the value to the next available value if any." +
                "You can turn this behavior on/off selectively on labels and values.";

            _selectedPropertyId = _id;
            rootEnum = AppendRootCategory(_id++, "SelectedProperty");
            rootEnum.Property.Comment = _helpText;
            rootEnum.Property.Tag = "Everything under this category concerns the property that is selected "+
                "in the PropertyGrid at the right.";

            this.Focus();

            EndUpdate();
        }

        void _targetPropertyGrid_DrawingManagerChanged(object sender, EventArgs e)
        {
            PropertyEnumerator propEnum = FindProperty(_useCustomDrawingManagerId);
            propEnum.Property.Value.SetValue(_targetPropertyGrid.DrawingManager == DrawManagers.CustomDrawManager);
        }

        void _targetPropertyGrid_RightToLeftChanged(object sender, EventArgs e)
        {
            _rightToLeft = (_targetPropertyGrid.RightToLeft == System.Windows.Forms.RightToLeft.Yes);
        }

        void OnTargetGridSizeChanged(object sender, EventArgs e)
        {
            if (_commentsHeightPropEnum != null)
            {
                (_commentsHeightPropEnum.Property.Value.Validator as PropertyValidatorMinMax).Max =
                    _targetPropertyGrid.Height - 2 * _targetPropertyGrid.BasicPropertyHeight - (_targetPropertyGrid.Toolbar != null ? _targetPropertyGrid.Toolbar.Height : 0);

                if ((SelectedPropertyEnumerator == _commentsHeightPropEnum) && (InPlaceControl != null))
                    // It will reinitialize the trackbar
                    (InPlaceControl as PropInPlaceTrackbar).OwnerPropertyEnumerator = _commentsHeightPropEnum;
            }
        }

        protected override void OnInPlaceCtrlVisible(InPlaceCtrlVisibleEventArgs e)
        {
            int id = e.PropertyEnum.Property.Id;

            if (id == _labelColumnWidthRatioId)
                (e.InPlaceCtrl as PropInPlaceTrackbar).RealtimeChange = true;
            else if (e.PropertyEnum == _commentsHeightPropEnum)
                (e.InPlaceCtrl as PropInPlaceTrackbar).RealtimeChange = true;

            base.OnInPlaceCtrlVisible(e);
        }

        protected override void OnValueValidation(ValueValidationEventArgs e)
        {
            if (((e.ValueValidationResult & PropertyValue.ValueValidationResult.ErrorCode) != 0) &&
                (ValueNotValidBehaviorMode == ValueNotValidBehaviorModes.KeepFocus))
            {
                if (!_errorVtp.IsTipDisplayed)
                {
                    if (_helpVtp.IsTipDisplayed)
                        _helpVtp.HideTip();

                    System.Windows.Forms.Control tb = (InPlaceControl as IInPlaceControl).TextBox;
                    if (tb != null)
                    {
                        _errorVisualTip.Text = e.Message;
                        _errorVisualTip.FooterText = "Need help? Press F1";
                        _errorVisualTip.Font = new Font(_errorVisualTip.Font, FontStyle.Regular);
                        _errorVisualTip.Title = e.PropertyEnum.Property.DisplayName;
                        _errorVtp.SetVisualTip(e.PropertyEnum, _errorVisualTip);
                        Rectangle rect = tb.RectangleToScreen(InPlaceControl.ClientRectangle);
                        _errorVtp.ShowTip(_errorVisualTip, rect, tb, Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyDown |
                            Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyPress |
                            Skybound.VisualTips.VisualTipDisplayOptions.HideOnLostFocus |
                            Skybound.VisualTips.VisualTipDisplayOptions.HideOnMouseDown |
                            Skybound.VisualTips.VisualTipDisplayOptions.ForwardEscapeKey |
                            Skybound.VisualTips.VisualTipDisplayOptions.HideOnTextChanged);
                    }
                }
            }

            base.OnValueValidation(e);
        }

        protected override void OnPropertyCreated(PropertyCreatedEventArgs e)
        {
            if (e.PropertyEnum.Parent.Property != null)
            {
                if (e.PropertyEnum.Parent.Property.Id == _fontId)
                {
                    Property property = e.PropertyEnum.Property;

                    if (property.Name.Equals("Size"))
                    {
                        property.Value.Validator = new PropertyValidatorMinMax((Single)8, (Single)14);
                        property.Comment = _helpText;
                        property.Tag = "This property, even if published by a TypeConverter whose code, and the one "+
                            "of the Font, is inaccessible, has been customized with an updown inplace control and a "+
                            "validator that allows values between 9 and 14 has been assigned (arbitrary values in "+
                            "this sample only).\n\nTry to enter another value and see how the custom error tip appears.";
                    }
                    else if ((property.Name.Equals("Bold")) || (property.Name.Equals("Italic")))
                    {
                        property.Comment = _helpText;
                        property.Tag = "This property, even if published by a TypeConverter whose code, and the one " +
                            "of the Font, is inaccessible, has been customized with a checkbox inplace control and " +
                            "the text has even been modified.";
                    }
                }
            }

            base.OnPropertyCreated(e);
        }

        protected override void OnPropertyChanged(VisualHint.SmartPropertyGrid.PropertyChangedEventArgs e)
        {
            int id = e.PropertyEnum.Property.Id;

            if (id == _drawingManagerId)
            {
                if (_targetPropertyGrid.DrawingManager == DrawManagers.DefaultDrawManager)
                    _targetPropertyGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                else
                    _targetPropertyGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;

                e.PropertyEnum.Property.Value.SetAttribute(new DefaultValueAttribute(e.PropertyEnum.Property.Value.GetValue()));
            }
            else if (id == _enabledId)
            {
                EnableProperty(FindProperty(_disableModeId), !_targetPropertyGrid.Enabled);
                EnableProperty(FindProperty(_disableModeGrayedOutId), !_targetPropertyGrid.Enabled &&
                    (_targetPropertyGrid.DisableMode != DisableModes.None));
            }
            else if (id == _disableModeId)
            {
                EnableProperty(FindProperty(_disableModeGrayedOutId),
                    (_targetPropertyGrid.DisableMode != DisableModes.None));
            }
            else if (id == _navigationKeyModeId)
            {
                EnableProperty(FindProperty(_tabKeyNavigationMode),
                    _targetPropertyGrid.NavigationKeyMode == NavigationKeyModes.TabKey);
            }
            else if (e.PropertyEnum == _commentsHeightPropEnum)
            {
                // A refresh is not enough because painting messages can't reach their destination well while
                // processing the mousemove messages on the trackbar and modifying the size of the target internal grid
                Win32Calls.RedrawWindow(_targetPropertyGrid.Handle, IntPtr.Zero, IntPtr.Zero,
                    Win32Calls.RDW_UPDATENOW | Win32Calls.RDW_INTERNALPAINT | Win32Calls.RDW_ALLCHILDREN);
            }
            else if (id == _propertyEnabledId)
            {
                _targetPropertyGrid.EnableProperty(_targetPropertyGrid.SelectedPropertyEnumerator,
                    (bool)e.PropertyEnum.Property.Value.GetValue());
            }
            else if (id == _propertyExpandedId)
            {
                _targetPropertyGrid.ExpandProperty(_targetPropertyGrid.SelectedPropertyEnumerator,
                    (bool)e.PropertyEnum.Property.Value.GetValue());
            }
            else if (id == _propertyManuallyDisabledId)
            {
                _targetPropertyGrid.SetManuallyDisabled(_targetPropertyGrid.SelectedPropertyEnumerator,
                    (bool)e.PropertyEnum.Property.Value.GetValue());
            }
            else if (id == _useCustomColorsId)
            {
                BeginUpdate();
                PropertyEnumerator childEnum = e.PropertyEnum.Parent.Children;
                childEnum.MoveNext();
                while (childEnum != childEnum.RightBound)
                {
                    if ((bool)e.PropertyEnum.Property.Value.GetValue())
                    {
                        childEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelEditList);
                        childEnum.Property.Look = new PropertyAlphaColorLook();
                    }
                    else
                    {
                        childEnum = RecreateProperty(childEnum);
                        childEnum.Property.Look = new PropertyColorLook();
                    }

                    childEnum.MoveNext();
                }
                EndUpdate();
            }
            else if (id == _useCustomDrawingManagerId)
            {
                bool value = (bool)e.PropertyEnum.Property.Value.GetValue();
                if (value)
                    _targetPropertyGrid.DrawManager = new CustomLightColorDrawManager();
                else
                    _targetPropertyGrid.DrawingManager = DrawManagers.DotnetDrawManager;
                _targetPropertyGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;

                PropertyEnumerator prevEnum = (PropertyEnumerator)e.PropertyEnum.GetSiblingEnumerator();
                prevEnum.MovePrev();
                
                BeginUpdate();
                RecreateProperty(prevEnum);
                HandleDefaultValues();
                EndUpdate();
            }

            base.OnPropertyChanged(e);
        }

        void OnTargetPropertySelected(object sender, PropertySelectedEventArgs e)
        {
            Property property = e.PropertyEnum.Property;
            if (property == null)
                return;

            PropertyEnumerator rootEnum = FindProperty(_selectedPropertyId);

            object states = SavePropertiesStates(PropertyStateFlags.None);

            BeginUpdate();  // Will keep the selected property in place
//            BeginVerticalFreeze();    // Would keep the first visible property in place

            ClearUnderProperty(rootEnum);

            PropertyEnumerator propEnum = AppendProperty(rootEnum, _id++, "DisplayName", property, "DisplayName", "");

            propEnum = AppendProperty(rootEnum, _id++, "Comment", property, "Comment", "");
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelMultilineEdit);
            propEnum.Property.Look = new PropertyMultilineEditLook();
 //           propEnum.Property.HeightMultiplier = 3;

            propEnum = AppendProperty(rootEnum, _id++, "BackColor", property, "BackColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(rootEnum, _id++, "ForeColor", property, "ForeColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            _propertyFontId = _id;
            propEnum = AppendProperty(rootEnum, _id++, "Font", property, "Font", "",
                new PropertyHideAttribute("GdiCharSet"),
                new PropertyHideAttribute("GdiVerticalFont"),
                new PropertyHideAttribute("Unit"),
                new PropertyFeelAttribute("Size", PropertyGrid.FeelEditUpDown));

            propEnum = AppendProperty(rootEnum, _id++, "Label icon index", property, "ImageIndex", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelEditUpDown);
            propEnum.Property.Value.Validator = new PropertyValidatorMinMax(-1, _targetPropertyGrid.ImageList.Images.Count - 1);
            propEnum.Property.Tag = "-1 means no icon. A positive number is the index of the icon in the image list "+
                "assigned to the grid.";

            propEnum = AppendProperty(rootEnum, _id++, "DisabledForeColor", property, "DisabledForeColor", "");
            propEnum.Property.Look = new PropertyColorLook();
            propEnum = AppendProperty(rootEnum, _id++, "ReadOnlyForeColor", property, "ReadOnlyForeColor", "");
            propEnum.Property.Look = new PropertyColorLook();

            _propertyEnabledId = _id;
            propEnum = AppendManagedProperty(rootEnum, _id++, "Enabled", typeof(bool), property.Enabled, _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "There is no Property.Enabled setter method in SPG and however this "+
                "property is editable. This is done by creating what is called a \"managed property\" that "+
                "keeps its value internal to the grid. When it changes, the new value is used to correctly "+
                "enable or disable the target property.";

            if (e.PropertyEnum.Children.Count > 0)
            {
                _propertyExpandedId = _id;
                propEnum = AppendManagedProperty(rootEnum, _id++, "Expanded", typeof(bool), property.Expanded, _helpText,
                    new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
                propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
                propEnum.Property.Look = new PropertyCheckboxLook();
                propEnum.Property.Tag = "There is no Property.Exapnded setter method in SPG and however this " +
                    "property is editable. This is done by creating what is called a \"managed property\" that " +
                    "keeps its value internal to the grid. When it changes, the new value is used to correctly " +
                    "expand or collapse the target property.";
            }
            else
                _propertyExpandedId = -1;

            _propertyManuallyDisabledId = _id;
            propEnum = AppendManagedProperty(rootEnum, _id++, "Manually disabled", typeof(bool), property.CanBeDisabledManually, _helpText,
                new PropertyValueDisplayedAsAttribute(new string[] { "", "" }));
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
            propEnum.Property.Look = new PropertyCheckboxLook();
            propEnum.Property.Tag = "When checked, it adds a checkbox at the left of the property label. It "+
                "lets the end-user enable/disable the property and all its descendants. This was done at "+
                "the top of this grid with the Commments property.";

            propEnum = AppendProperty(rootEnum, _id++, "Height multiplier", property, "HeightMultiplier", _helpText);
            propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelEditUpDown);

            PropertyValue propertyValue = property.Value;
            if (propertyValue != null)
            {
                _propertyValueId = _id;
                PropertyEnumerator subcatEnum = AppendSubCategory(rootEnum, _id++, "Value");

                propEnum = AppendProperty(subcatEnum, _id++, "BackColor", propertyValue, "BackColor", "");
                propEnum.Property.Look = new PropertyColorLook();
                propEnum = AppendProperty(subcatEnum, _id++, "ForeColor", propertyValue, "ForeColor", "");
                propEnum.Property.Look = new PropertyColorLook();
                _propertyValueFontId = _id;
                propEnum = AppendProperty(subcatEnum, _id++, "Font", propertyValue, "Font", "",
                    new PropertyHideAttribute("GdiCharSet"),
                    new PropertyHideAttribute("GdiVerticalFont"),
                    new PropertyHideAttribute("Unit"),
                    new PropertyFeelAttribute("Size", PropertyGrid.FeelEditUpDown));
                propEnum = AppendProperty(subcatEnum, _id++, "DisabledForeColor", propertyValue, "DisabledForeColor", "");
                propEnum.Property.Look = new PropertyColorLook();
                propEnum = AppendProperty(subcatEnum, _id++, "ReadOnlyForeColor", propertyValue, "ReadOnlyForeColor", "");
                propEnum.Property.Look = new PropertyColorLook();

                propEnum = AppendProperty(subcatEnum, _id++, "ReadOnly", propertyValue, "ReadOnly", _helpText,
                    new PropertyValueDisplayedAsAttribute(new string[] { "Yes", "No" }));
                propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelCheckbox);
                propEnum.Property.Look = new PropertyCheckboxLook();
                propEnum.Property.Tag = "With this property you can change the readonly state of properties "+
                    "at runtime. Try it on parent properties. It will sometimes propagate the state, "+
                    "sometimes not, depending on the link between parents and children.\n\nNote that you "+
                    "can't change the readonly status of a property which has a hardcoded [Readonly] attribute "+
                    "because the default PropertyDescriptor won't let you use the setter method anyway.";

                propEnum = AppendProperty(subcatEnum, _id++, "Show icon from", propertyValue, "ImageSource", "");

                propEnum = AppendProperty(subcatEnum, _id++, "Icon index", propertyValue, "ImageIndex", "");
                propEnum.Property.Feel = GetRegisteredFeel(PropertyGrid.FeelEditUpDown);
                propEnum.Property.Value.Validator = new PropertyValidatorMinMax(0, _targetPropertyGrid.ImageList.Images.Count-1);
            }
            else
                _propertyValueFontId = -1;

            RestorePropertiesStates(states);

            EndUpdate();
//            EndVerticalFreeze();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
        {
            System.Windows.Forms.Keys key = (keyData & System.Windows.Forms.Keys.KeyCode);

            if (key == System.Windows.Forms.Keys.F1)
            {
                if ((SelectedPropertyEnumerator != RightBound) && (SelectedPropertyEnumerator.Property.Tag != null))
                {
                    if (_errorVtp.IsTipDisplayed)
                        _errorVtp.HideTip();

                    _helpVisualTip.Title = SelectedPropertyEnumerator.Property.DisplayName;
                    _helpVisualTip.Text = (string)SelectedPropertyEnumerator.Property.Tag;
                    _helpVisualTip.Font = new Font(_helpVisualTip.Font, FontStyle.Regular);
                    _helpVtp.SetVisualTip(SelectedPropertyEnumerator, _helpVisualTip);

                    Rectangle rect = SelectedPropertyEnumerator.Property.GetLabelColumnRect(
                        GetItemRect(SelectedPropertyEnumerator), SelectedPropertyEnumerator);
                    rect = InternalGrid.RectangleToScreen(rect);
                    System.Windows.Forms.Control control;
                    if (InternalGrid.Focused || (InPlaceControl == null))
                        control = InternalGrid;
                    else
                        control = System.Windows.Forms.Control.FromHandle(GetFocus());

                    _helpVtp.ShowTip(_helpVisualTip, rect, control,
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyDown |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnKeyPress |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnLostFocus |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnMouseDown |
                        Skybound.VisualTips.VisualTipDisplayOptions.ForwardEscapeKey |
                        Skybound.VisualTips.VisualTipDisplayOptions.HideOnTextChanged);

                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnMouseEnterProperty(SPGMouseEventArgs e)
        {
            e.PropertyEnumerator.Property.BackColor = _hoveredPropertyBackColor;
            if (e.PropertyEnumerator.Property.Value != null)
                e.PropertyEnumerator.Property.Value.BackColor = GridBackColor;
            base.OnMouseEnterProperty(e);
        }

        protected override void OnMouseLeaveProperty(SPGMouseEventArgs e)
        {
            e.PropertyEnumerator.Property.BackColor = Color.Empty;
            if (e.PropertyEnumerator.Property.Value != null)
                e.PropertyEnumerator.Property.Value.BackColor = Color.Empty;
            base.OnMouseLeaveProperty(e);
        }
    }
}
