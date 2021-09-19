package crc642e1c7a98bdb5c44a;


public class PopupRenderer
	extends android.app.Dialog
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnCancelListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_show:()V:GetShowHandler\n" +
			"n_onCancel:(Landroid/content/DialogInterface;)V:GetOnCancel_Landroid_content_DialogInterface_Handler:Android.Content.IDialogInterfaceOnCancelListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Xamarin.CommunityToolkit.UI.Views.PopupRenderer, Xamarin.CommunityToolkit", PopupRenderer.class, __md_methods);
	}


	public PopupRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == PopupRenderer.class)
			mono.android.TypeManager.Activate ("Xamarin.CommunityToolkit.UI.Views.PopupRenderer, Xamarin.CommunityToolkit", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void show ()
	{
		n_show ();
	}

	private native void n_show ();


	public void onCancel (android.content.DialogInterface p0)
	{
		n_onCancel (p0);
	}

	private native void n_onCancel (android.content.DialogInterface p0);

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
