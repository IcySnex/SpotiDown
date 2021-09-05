package mono.androidx.preference;


public class PreferenceManager_OnDisplayPreferenceDialogListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.PreferenceManager.OnDisplayPreferenceDialogListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDisplayPreferenceDialog:(Landroidx/preference/Preference;)V:GetOnDisplayPreferenceDialog_Landroidx_preference_Preference_Handler:AndroidX.Preference.PreferenceManager/IOnDisplayPreferenceDialogListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.PreferenceManager+IOnDisplayPreferenceDialogListenerImplementor, Xamarin.AndroidX.Preference", PreferenceManager_OnDisplayPreferenceDialogListenerImplementor.class, __md_methods);
	}


	public PreferenceManager_OnDisplayPreferenceDialogListenerImplementor ()
	{
		super ();
		if (getClass () == PreferenceManager_OnDisplayPreferenceDialogListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.PreferenceManager+IOnDisplayPreferenceDialogListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public void onDisplayPreferenceDialog (androidx.preference.Preference p0)
	{
		n_onDisplayPreferenceDialog (p0);
	}

	private native void n_onDisplayPreferenceDialog (androidx.preference.Preference p0);

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
