﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    public partial class CursorIndicator : Form
    {
        IMouseRawEventProvider m;
        SettingsStore s;
        float factorX = 1;
        float factorY = 1;
        float defaultDpi = 96;
        Point newLocation = new Point();

        public CursorIndicator(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            FormClosed += CursorIndicator_FormClosed;

            NativeMethodsSWP.SetWindowTopMost(this.Handle);
            SetFormStyles();

            m.MouseEvent += m_MouseEvent;
            Paint += CursorIndicator_Paint;
            s.settingChanged += settingChanged;

            BackColor = Color.Lavender;
            TransparencyKey = Color.Lavender;
        }

        void CursorIndicator_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(s.CursorIndicatorColor, 7);
            g.FillEllipse(p.Brush, 0, 0, s.CursorIndicatorSize.Width, s.CursorIndicatorSize.Height);

            factorX = g.DpiX / defaultDpi;
            factorY = g.DpiY / defaultDpi;

            Console.WriteLine($"DPI X: {g.DpiX}");
            Console.WriteLine($"DPI Y: {g.DpiY}");

            Console.WriteLine($"factor X: {factorX}");
            Console.WriteLine($"factor Y: {factorY}");
            //UpdatePosition();
        }

        Point cursorPosition;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            cursorPosition = raw_e.Position;
            if(raw_e.Action == MouseAction.Move)
                UpdatePosition();
        }

        void CursorIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ( m != null )
                m.MouseEvent -= m_MouseEvent;
            if ( s != null )
                s.settingChanged -= settingChanged;
            m = null;
            s = null;
        }

        void SetFormStyles()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = s.CursorIndicatorOpacity;
            NativeMethodsGWL.ClickThrough(this.Handle);
            NativeMethodsGWL.HideFromAltTab(this.Handle);

            UpdateSize();
            UpdatePosition();
        }

        

        void UpdateSize()
        {
            this.Size = s.CursorIndicatorSize;
            this.Invalidate(new Rectangle(0, 0, this.Size.Width, this.Size.Height));
        }

        void UpdatePosition()
        {
            newLocation.X = (int)((cursorPosition.X - this.Size.Width / 2) * factorX);
            newLocation.Y = (int)((cursorPosition.Y - this.Size.Height / 2) * factorY);
            this.Location = newLocation;
        }

        private void settingChanged(SettingsChangedEventArgs e)
        {
            switch (e.Name)
            {
                case "EnableCursorIndicator":
                    break;
                case "CursorIndicatorOpacity":
                    this.Opacity = s.CursorIndicatorOpacity;
                    break;
                case "CursorIndicatorSize":
                    UpdateSize();
                    break;
                case "CursorIndicatorColor":
                    UpdateSize(); // invalidates
                    break;
            }
        }


    }
}
