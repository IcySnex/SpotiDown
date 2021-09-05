package mono.androidx.preference;


public class PreferenceManager_OnNavigateToScreenListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.PreferenceManager.OnNavigateToScreenListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onNavigateToScreen:(Landroidx/preference/PreferenceScreen;)V:GetOnNavigateToScreen_Landroidx_preference_PreferenceScreen_Handler:AndroidX.Preference.PreferenceManager/IOnNavigateToScreenListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.PreferenceManager+IOnNavigateToScreenListenerImplementor, Xamarin.AndroidX.Preference", PreferenceManager_OnNavigateToScreenListenerImplementor.class, __md_methods);
	}


	public PreferenceManager_OnNavigateToScreenListenerImplementor ()
	{
		super ();
		if (getClass () == PreferenceManager_OnNavigateToScreenListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.PreferenceManager+IOnNavigateToScreenListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public void onNavigateToScreen (androidx.preference.PreferenceScreen p0)
	{
		n_onNavigateToScreen (p0);
	}

	private native void n_onNavigateToScreen (androidx.preference.PreferenceScreen p0);

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
