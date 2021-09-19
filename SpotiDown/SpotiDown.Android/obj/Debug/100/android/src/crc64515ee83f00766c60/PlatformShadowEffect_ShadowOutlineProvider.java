package crc64515ee83f00766c60;


public class PlatformShadowEffect_ShadowOutlineProvider
	extends android.view.ViewOutlineProvider
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getOutline:(Landroid/view/View;Landroid/graphics/Outline;)V:GetGetOutline_Landroid_view_View_Landroid_graphics_Outline_Handler\n" +
			"";
		mono.android.Runtime.register ("Xamarin.CommunityToolkit.Android.Effects.PlatformShadowEffect+ShadowOutlineProvider, Xamarin.CommunityToolkit", PlatformShadowEffect_ShadowOutlineProvider.class, __md_methods);
	}


	public PlatformShadowEffect_ShadowOutlineProvider ()
	{
		super ();
		if (getClass () == PlatformShadowEffect_ShadowOutlineProvider.class)
			mono.android.TypeManager.Activate ("Xamarin.CommunityToolkit.Android.Effects.PlatformShadowEffect+ShadowOutlineProvider, Xamarin.CommunityToolkit", "", this, new java.lang.Object[] {  });
	}

	public PlatformShadowEffect_ShadowOutlineProvider (float p0, float p1, float p2)
	{
		super ();
		if (getClass () == PlatformShadowEffect_ShadowOutlineProvider.class)
			mono.android.TypeManager.Activate ("Xamarin.CommunityToolkit.Android.Effects.PlatformShadowEffect+ShadowOutlineProvider, Xamarin.CommunityToolkit", "System.Single, mscorlib:System.Single, mscorlib:System.Single, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void getOutline (android.view.View p0, android.graphics.Outline p1)
	{
		n_getOutline (p0, p1);
	}

	private native void n_getOutline (android.view.View p0, android.graphics.Outline p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
