package mono.androidx.preference;


public class PreferenceManager_OnPreferenceTreeClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.PreferenceManager.OnPreferenceTreeClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPreferenceTreeClick:(Landroidx/preference/Preference;)Z:GetOnPreferenceTreeClick_Landroidx_preference_Preference_Handler:AndroidX.Preference.PreferenceManager/IOnPreferenceTreeClickListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.PreferenceManager+IOnPreferenceTreeClickListenerImplementor, Xamarin.AndroidX.Preference", PreferenceManager_OnPreferenceTreeClickListenerImplementor.class, __md_methods);
	}


	public PreferenceManager_OnPreferenceTreeClickListenerImplementor ()
	{
		super ();
		if (getClass () == PreferenceManager_OnPreferenceTreeClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.PreferenceManager+IOnPreferenceTreeClickListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public boolean onPreferenceTreeClick (androidx.preference.Preference p0)
	{
		return n_onPreferenceTreeClick (p0);
	}

	private native boolean n_onPreferenceTreeClick (androidx.preference.Preference p0);

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
