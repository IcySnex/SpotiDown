package mono.androidx.preference;


public class Preference_OnPreferenceChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.Preference.OnPreferenceChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPreferenceChange:(Landroidx/preference/Preference;Ljava/lang/Object;)Z:GetOnPreferenceChange_Landroidx_preference_Preference_Ljava_lang_Object_Handler:AndroidX.Preference.Preference/IOnPreferenceChangeListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.Preference+IOnPreferenceChangeListenerImplementor, Xamarin.AndroidX.Preference", Preference_OnPreferenceChangeListenerImplementor.class, __md_methods);
	}


	public Preference_OnPreferenceChangeListenerImplementor ()
	{
		super ();
		if (getClass () == Preference_OnPreferenceChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.Preference+IOnPreferenceChangeListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public boolean onPreferenceChange (androidx.preference.Preference p0, java.lang.Object p1)
	{
		return n_onPreferenceChange (p0, p1);
	}

	private native boolean n_onPreferenceChange (androidx.preference.Preference p0, java.lang.Object p1);

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
