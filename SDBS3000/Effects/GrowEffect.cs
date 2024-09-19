using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SDBS3000.Effects
{
    public class GrowEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrowEffect), 0);
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
            "Amount",
            typeof(double),
            typeof(GrowEffect),
            new UIPropertyMetadata(((double)(3.5D)), PixelShaderConstantCallback(0))
        );

        public GrowEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(
                "/SDBS3000.Effects;component/Assets/Shaders/GrowEffect.ps",
                UriKind.Relative
            );
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(AmountProperty);
        }

        public Brush Input
        {
            get { return ((Brush)(this.GetValue(InputProperty))); }
            set { this.SetValue(InputProperty, value); }
        }
        public double Amount
        {
            get { return ((double)(this.GetValue(AmountProperty))); }
            set { this.SetValue(AmountProperty, value); }
        }
    }
}
