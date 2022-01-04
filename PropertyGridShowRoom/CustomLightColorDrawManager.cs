using System;
using VisualHint.SmartPropertyGrid;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WindowsApplication
{
    public class CustomLightColorDrawManager : LightColorDrawManager
    {
        public override void DrawPlusMinusSign(Graphics graphics, Control control, Rectangle itemRect, PropertyEnumerator propEnum)
        {
            Property property = propEnum.Property;

            Rectangle signRect = property.GetSignRect(itemRect, propEnum);
            if (Rectangle.Intersect(Grid.TransformRectIfRTL(signRect), Rectangle.Round(graphics.ClipBounds)).IsEmpty)
                return;

            // Draw a square
            using (Brush brush = new SolidBrush(Color.White))
            {
                Rectangle fillRect = signRect;
                fillRect = Grid.TransformRectIfRTL(fillRect);
                graphics.FillRectangle(brush, fillRect);
            }

            using (Pen pen = new Pen(Color.FromArgb(120, 152, 181)))
            {
                signRect = Grid.TransformRectIfRTL(signRect);
                graphics.DrawRectangle(pen, signRect);
            }

            // Find the center of this square
            Point ptCenter = new Point(signRect.X + signRect.Width / 2, signRect.Y + signRect.Height / 2);

            using (Pen pen = new Pen(Color.Black))
            {
                graphics.DrawLine(pen, ptCenter.X - 2, ptCenter.Y, ptCenter.X + 2, ptCenter.Y);

                // If the children are hidden, we have to draw the +
                if (!property.Expanded)
                {
                    // Draw a vertical bar (plus)
                    graphics.DrawLine(pen, ptCenter.X, ptCenter.Y - 2, ptCenter.X, ptCenter.Y + 2);
                }
            }
        }

        public override void DrawPropertyHyperlinkLabelText(Graphics graphics, Rectangle labelColumnRect, Color textColor, PropertyVisibleDeepEnumerator enumSelf)
        {
            textColor = Color.FromArgb(192, 192, 192);

            labelColumnRect.X += Grid.GlobalTextMargin;
            labelColumnRect.Width -= 2 * Grid.GlobalTextMargin;
            labelColumnRect.Height--;
            labelColumnRect = Grid.TransformRectIfRTL(labelColumnRect);

            // Set the font to underline
            using (Font underlinedFont = new Font(enumSelf.Property.Font, FontStyle.Underline))
            {
                string str = enumSelf.Property.DisplayName;

                // Draw the text, clipped by the given area
                Size textSize = Win32Calls.GetTextExtent(graphics, str, underlinedFont);

                if (textSize.Width <= labelColumnRect.Width)
                    Win32Calls.DrawText(graphics, str, ref labelColumnRect, underlinedFont, textColor,
                        Win32Calls.DT_SINGLELINE | Win32Calls.DT_VCENTER | Win32Calls.DT_NOPREFIX |
                        (Grid.RightToLeft == RightToLeft.Yes ? Win32Calls.DT_RTLREADING : Win32Calls.DT_RIGHT));
                else
                    Win32Calls.DrawText(graphics, str, ref labelColumnRect, underlinedFont, textColor,
                        Win32Calls.DT_LEFT | Win32Calls.DT_SINGLELINE | Win32Calls.DT_VCENTER | Win32Calls.DT_END_ELLIPSIS |
                        Win32Calls.DT_NOPREFIX |
                        (Grid.RightToLeft == RightToLeft.Yes ? Win32Calls.DT_RTLREADING : Win32Calls.DT_RIGHT));
            }

            if (enumSelf.Property.Selected)
            {
                labelColumnRect.X -= Grid.GlobalTextMargin / 2;
                labelColumnRect.Height--;
                labelColumnRect.Width = labelColumnRect.Right - labelColumnRect.Left + Grid.GlobalTextMargin;
                using (Pen pen = new Pen((Grid.ContainsFocus ?
                    Color.FromArgb(208, 208, 232) : Color.FromArgb(246, 217, 113)), 2.0f))
                    graphics.DrawRectangle(pen, labelColumnRect);
            }
        }

        public override void DrawPropertyLabelText(Graphics graphics, Rectangle labelColumnRect,
            Rectangle strRect, Color textColor, PropertyVisibleDeepEnumerator enumSelf)
        {
            Font font = GetPropertyLabelFont(enumSelf.Property);
            if ((enumSelf.Parent.Property != null) && (enumSelf.Parent.Property.Value != null))
                textColor = Color.FromArgb(128, 128, 128);
            else if (enumSelf.Property.Value.ReadOnly)
                textColor = enumSelf.Property.ReadOnlyForeColor;
            else
                textColor = Color.FromArgb(192, 192, 192);

            strRect = Grid.TransformRectIfRTL(strRect);

            if (Rectangle.Intersect(strRect, Rectangle.Round(graphics.ClipBounds)).IsEmpty)
                return;

            strRect.Height = strRect.Height / enumSelf.Property.HeightMultiplier;

            // Draw the text, clipped by the given area
            Win32Calls.DrawText(graphics, enumSelf.Property.DisplayName, ref strRect, font, textColor,
                Win32Calls.DT_SINGLELINE | Win32Calls.DT_VCENTER | Win32Calls.DT_NOPREFIX |
                ((Grid.EllipsisMode & VisualHint.SmartPropertyGrid.PropertyGrid.EllipsisModes.EllipsisOnLabels) != 0 ? Win32Calls.DT_END_ELLIPSIS : 0) |
                (Grid.RightToLeft == RightToLeft.Yes ? Win32Calls.DT_RIGHT | Win32Calls.DT_RTLREADING : 0));

            if (enumSelf.Property.Selected)
            {
                strRect = Grid.TransformRectIfRTL(strRect);
                strRect.X -= Grid.GlobalTextMargin / 2;
                strRect.Height--;
                strRect.Width = labelColumnRect.Right - strRect.Left;
                strRect = Grid.TransformRectIfRTL(strRect);

                using (Pen pen = new Pen((Grid.ContainsFocus ?
                    Color.FromArgb(208, 208, 232) : Color.FromArgb(246, 217, 113)), 2.0f))
                        graphics.DrawRectangle(pen, strRect);
            }
        }

        public override void DrawPropertyLabelBackground(Graphics graphics, Rectangle labelRect, PropertyEnumerator enumSelf)
        {
            Rectangle fillRect = Rectangle.Intersect(Grid.TransformRectIfRTL(labelRect), Rectangle.Round(graphics.ClipBounds));
            if (fillRect.IsEmpty)
                return;

            using(Brush brush = new SolidBrush(Grid.GridBackColor))
                graphics.FillRectangle(brush, Grid.TransformRectIfRTL(labelRect));

            Rectangle textRect = enumSelf.Property.GetLabelTextRect(labelRect, enumSelf);
            int margin = enumSelf.Property.ParentGrid.GlobalTextMargin;
            textRect.X -= margin / 2;
            textRect.Width = labelRect.Right - textRect.Left - 1;
            textRect.Y += 1;
            textRect.Height -= 3;

            textRect = Grid.TransformRectIfRTL(textRect);
            using (Brush brush = new SolidBrush(enumSelf.Property.IsSetBackColor ? enumSelf.Property.BackColor : Grid.GridBackColor))
                graphics.FillRectangle(brush, textRect);
        }

        public override Color GetValueBackgroundColor(PropertyEnumerator propEnum)
        {
            return (propEnum.Property.IsSetBackColor ? propEnum.Property.BackColor : Grid.GridBackColor);
        }

        public override void DrawCategoryLabelBackground(Graphics graphics, Rectangle rect, PropertyEnumerator enumSelf)
        {
            Rectangle fillRect = Rectangle.Intersect(Grid.TransformRectIfRTL(rect), Rectangle.Round(graphics.ClipBounds));
            if (fillRect.IsEmpty)
                return;

            Rectangle labelRect = enumSelf.Property.GetLabelColumnRect(rect, enumSelf);
            rect.Y++;
            rect.Height -= 3;

            fillRect = rect;
            int margin = enumSelf.Property.ParentGrid.GlobalTextMargin;
            fillRect.X = labelRect.Left + margin / 2;
            fillRect.Width = rect.Right - fillRect.Left;
            fillRect = Grid.TransformRectIfRTL(fillRect);

            Brush brush;
            if (Grid.RightToLeft == RightToLeft.Yes)
                brush = new LinearGradientBrush(fillRect, CategoryBkgColor2, CategoryBkgColor1, 0.0f);
            else
                brush = new LinearGradientBrush(fillRect, CategoryBkgColor1, CategoryBkgColor2, 0.0f);
            
            graphics.FillRectangle(brush, fillRect);

            brush.Dispose();
        }

        public override void DrawCategoryValue(Graphics graphics, Rectangle valueRect, Color textColor, PropertyEnumerator enumSelf)
        {
            // Set the font to bold for categories
            Font font;
            if ((enumSelf.Property is SubCategory) || !UseBoldFontForCategories)
                font = Grid.Font;
            else
                font = new Font(Grid.Font, FontStyle.Bold);

            // Draw the category info text
            //----------------------------

            valueRect = Grid.TransformRectIfRTL(valueRect);

            Win32Calls.DrawText(graphics, ((RootCategory)enumSelf.Property).ValueText, ref valueRect,
                font, Color.FromArgb(192, 192, 192),
                Win32Calls.DT_SINGLELINE | Win32Calls.DT_VCENTER | Win32Calls.DT_NOPREFIX |
                ((Grid.EllipsisMode & VisualHint.SmartPropertyGrid.PropertyGrid.EllipsisModes.EllipsisOnValues) != 0 ? Win32Calls.DT_END_ELLIPSIS : 0) |
                (Grid.RightToLeft == RightToLeft.Yes ? Win32Calls.DT_RIGHT | Win32Calls.DT_RTLREADING : 0));

            if (!(enumSelf.Property is SubCategory) && UseBoldFontForCategories)
                font.Dispose();
        }

        public override void DrawCategoryLabelText(Graphics graphics, Rectangle itemRect, Rectangle labelRect, Color textColor, PropertyEnumerator enumSelf)
        {
            base.DrawCategoryLabelText(graphics, itemRect, labelRect, Color.FromArgb(192, 192, 192), enumSelf);

            if (enumSelf.Property.Selected)
            {
                labelRect.X -= Grid.GlobalTextMargin / 2;
                labelRect.Width = itemRect.Right - labelRect.Left - Grid.GlobalTextMargin / 2;
                labelRect.Height--;
                labelRect = Grid.TransformRectIfRTL(labelRect);
                using (Pen pen = new Pen(Grid.ContainsFocus ?
                    Color.FromArgb(208, 208, 232) : Color.FromArgb(246, 217, 113), 2.0f))
                    graphics.DrawRectangle(pen, labelRect);
            }
        }

        public override void DrawSubCategoryLabelBackground(Graphics graphics, int labelRectX, Rectangle rect, PropertyEnumerator enumSelf)
        {
            Rectangle fillRect = Rectangle.Intersect(Grid.TransformRectIfRTL(rect), Rectangle.Round(graphics.ClipBounds));
            if (fillRect.IsEmpty)
                return;

            Rectangle labelRect = enumSelf.Property.GetLabelColumnRect(rect, enumSelf);
            rect.Y++;
            rect.Height -= 3;

            int margin = enumSelf.Property.ParentGrid.GlobalTextMargin;
            fillRect = rect;
            Rectangle signRect = enumSelf.Property.GetSignRect(rect, enumSelf);
            if (signRect != Rectangle.Empty)
                fillRect.X = signRect.Right + margin / 2;
            else
                fillRect.X = labelRect.Left + margin / 2;
            fillRect.Width = rect.Right - fillRect.Left;
            fillRect = Grid.TransformRectIfRTL(fillRect);

            Brush brush;
            if (Grid.RightToLeft == RightToLeft.Yes)
                brush = new LinearGradientBrush(fillRect, SubCategoryBkgColor2, SubCategoryBkgColor1, 0.0f);
            else
                brush = new LinearGradientBrush(fillRect, SubCategoryBkgColor1, SubCategoryBkgColor2, 0.0f);

            graphics.FillRectangle(brush, fillRect);

            brush.Dispose();
        }

        public override void DrawSubCategoryLabelText(Graphics graphics, Rectangle itemRect, Rectangle labelRect, Color textColor, PropertyEnumerator enumSelf)
        {
            base.DrawSubCategoryLabelText(graphics, itemRect, labelRect, Color.FromArgb(192, 192, 192), enumSelf);

            if (enumSelf.Property.Selected)
            {
                labelRect.X -= Grid.GlobalTextMargin / 2;
                labelRect.Width = itemRect.Right - labelRect.Left - Grid.GlobalTextMargin / 2;
                labelRect.Height--;
                labelRect = Grid.TransformRectIfRTL(labelRect);
                using (Pen pen = new Pen(Grid.ContainsFocus ?
                    Color.FromArgb(208, 208, 232) : Color.FromArgb(246, 217, 113), 2.0f))
                    graphics.DrawRectangle(pen, labelRect);
            }
        }

        public override string GetDisplayName(Property property, string displayName)
        {
            if (property.Value != null)
            {
                PropertyEnumerator propEnum = property.Value.OwnerEnumerator;

                if ((propEnum.Parent.Property != null) && (propEnum.Parent.Property.Value != null))
                    return "- " + displayName;
            }

            return base.GetDisplayName(property, displayName);
        }

        public override Font GetPropertyLabelFont(Property property)
        {
            PropertyEnumerator propEnum = property.Value.OwnerEnumerator;

            if ((propEnum.Parent.Property == null) || (propEnum.Parent.Property.Value == null))
                return base.GetPropertyLabelFont(property);

            return new Font(property.Font, FontStyle.Italic);
        }
    }
}
