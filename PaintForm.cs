using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class PaintForm : System.Windows.Forms.Form
    {
        public PaintForm()
        {
            InitializeComponent();
            sazeSide = 50;
            rectangles = new List<Rectangle>();
            editModeBoolean = false;
            sizeModeBoolean = false;
            indexRectangle = -1;
            graphics = CreateGraphics();
        }

        private Graphics graphics;
        private List<Rectangle> rectangles;
        private int indexRectangle;
        private bool editModeBoolean;
        private bool sizeModeBoolean;
        private int startX;
        private int startY;
        private int sazeSide;

        /// <summary>
        /// Перерисовка всех элементов
        /// </summary>
        private void PaintForm_Paint(object sender, PaintEventArgs e)
        {
            if (editModeBoolean)
            {
                ReDrawEdit(indexRectangle);
            }
            else
            {
                foreach (var itemRectangles in rectangles)
                {
                    graphics.FillRectangle(new SolidBrush(Color.Purple), itemRectangles);
                }
            }
        }

        /// <summary>
        /// Прорисовка новой фигуры или переход в режим редактирования
        /// </summary>
        private void PaintForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (clickRectangle(e) == -1 && !editModeBoolean)
            {
                rectangles.Add(new Rectangle(e.Location.X - sazeSide / 2, e.Location.Y - sazeSide / 2, sazeSide, sazeSide));
                graphics.FillRectangle(new SolidBrush(Color.Purple), rectangles[rectangles.Count - 1]);
                return;
            }
            if (clickRectangle(e) != -1 && !editModeBoolean)
            {
                editMode(clickRectangle(e));
                return;
            }
        }

        /// <summary>
        /// Подготовка фигуры к изменениям
        /// </summary>
        private void PaintForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (indexRectangle != -1)
            {
                startX = e.Location.X;
                startY = e.Location.Y;
                var graphicPath = new GraphicsPath();
                graphicPath.AddRectangle(rectangles[indexRectangle]);
                if (graphicPath.IsOutlineVisible(e.Location, new Pen(Color.Purple, 2)))
                {
                    sizeModeBoolean = true;
                }
            }
        }

        /// <summary>
        /// Перетаскивание и растягивание
        /// </summary>
        private void PaintForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && indexRectangle != -1)
            {
                var item = rectangles[indexRectangle];
                if (sizeModeBoolean)
                {
                    var newWidth = e.X - startX;
                    var newHeight = e.Y - startY;
                    if (newWidth >= 0)
                    {
                        item.Width = newWidth;
                    }
                    else
                    {
                        item.X = e.X;
                        item.Width = Math.Abs(newWidth);
                    }
                    if (newHeight >= 0)
                    {
                        item.Height = newHeight;
                    }
                    else
                    {
                        item.Y = e.Y;
                        item.Height = Math.Abs(newHeight);
                    }
                }
                else
                {
                    item.X = e.Location.X - sazeSide / 2;
                    item.Y = e.Location.Y - sazeSide / 2;
                }
                rectangles[indexRectangle] = item;
                graphics.FillRectangle(new SolidBrush(Color.Purple), rectangles[indexRectangle]);
                Refresh();
                ReDrawEdit(indexRectangle);
            }
        }

        /// <summary>
        /// Прекращение режима изменения размера
        /// </summary>
        private void PaintForm_MouseUp(object sender, MouseEventArgs e)
        {
            sizeModeBoolean = false;
        }

        /// <summary>
        /// Прекращение режима редактирования
        /// </summary>
        private void PaintForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (clickRectangle(e) == -1 && editModeBoolean)
            {
                editModeBoolean = false;
                foreach (var itemRectangles in rectangles)
                {
                    graphics.FillRectangle(new SolidBrush(Color.Purple), itemRectangles);
                }
            }
        }

        /// <summary>
        /// Начало режима редактирования
        /// </summary>
        /// <param name="indexItem"> Индекс фигуры</param>
        private void editMode(int indexItem)
        {
            editModeBoolean = true;
            graphics.Clear(BackColor);
            ReDrawEdit(indexItem);
            indexRectangle = indexItem;
        }

        /// <summary>
        /// Поиск индекса выбранной фигуры
        /// </summary>
        private int clickRectangle(MouseEventArgs e)
        {
            var result = -1;
            foreach (var item in rectangles)
            {
                if (item.Contains(e.X, e.Y))
                {
                    result = rectangles.IndexOf(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Перерисовка элементов для режима редактирования
        /// </summary>
        /// <param name="index">Индекс выбранной фигуры</param>
        private void ReDrawEdit(int index)
        {
            foreach (var itemRectangles in rectangles)
            {
                if (itemRectangles != rectangles[index])
                {
                    graphics.FillRectangle(new SolidBrush(Color.Gray), itemRectangles);
                }
                else
                {
                    graphics.FillRectangle(new SolidBrush(Color.Purple), itemRectangles);
                }
            }
        }
    }
}
