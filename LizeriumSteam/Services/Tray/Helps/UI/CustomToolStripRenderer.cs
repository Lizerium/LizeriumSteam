/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 апреля 2026 12:48:19
 * Version: 1.0.15
 */

using System.Drawing;
using System.Windows.Forms;

namespace LizeriumSteam.Services.Tray.Helps.UI
{
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            // Заливаем фон цветом из элемента
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), e.Item.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), e.Item.Bounds);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.ForeColor;
            base.OnRenderItemText(e);
        }
    }
}
