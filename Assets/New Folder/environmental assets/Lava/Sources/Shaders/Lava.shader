Shader "Lava" {

Properties
{
   _MainTex ("Texture 1  (vertex A  = white)", 2D) = ""
   _BlendTex ("Texture 2  (vertex A = black)", 2D) = ""
   

   
	_DisplacementHeightMap ("1st displacement", 2D) = "" {}
	_SecondDisplacementHeightMap ("2nd displacement", 2D) = "" {} 
}

SubShader
{
   BindChannels
   {
      Bind "vertex", vertex
      Bind "color", color
      Bind "texcoord", texcoord
   }
   
   Pass
   {
      SetTexture [_MainTex]
	  SetTexture [_BlendTex]	  {
			combine previous lerp(primary) texture
	  }
   }
}

 
}