package mono.androidx.preference;


public class Preference_OnPreferenceClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.Preference.OnPreferenceClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPreferenceClick:(Landroidx/preference/Preference;)Z:GetOnPreferenceClick_Landroidx_preference_Preference_Handler:AndroidX.Preference.Preference/IOnPreferenceClickListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.Preference+IOnPreferenceClickListenerImplementor, Xamarin.AndroidX.Preference", Preference_OnPreferenceClickListenerImplementor.class, __md_methods);
	}


	public Preference_OnPreferenceClickListenerImplementor ()
	{
		super ();
		if (getClass () == Preference_OnPreferenceClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.Preference+IOnPreferenceClickListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public boolean onPreferenceClick (androidx.preference.Preference p0)
	{
		return n_onPreferenceClick (p0);
	}

	private native boolean n_onPreferenceClick (androidx.preference.Preference p0);

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
