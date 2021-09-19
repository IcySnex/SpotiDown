package mono.androidx.preference;


public class PreferenceGroup_OnExpandButtonClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.PreferenceGroup.OnExpandButtonClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onExpandButtonClick:()V:GetOnExpandButtonClickHandler:AndroidX.Preference.PreferenceGroup/IOnExpandButtonClickListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.PreferenceGroup+IOnExpandButtonClickListenerImplementor, Xamarin.AndroidX.Preference", PreferenceGroup_OnExpandButtonClickListenerImplementor.class, __md_methods);
	}


	public PreferenceGroup_OnExpandButtonClickListenerImplementor ()
	{
		super ();
		if (getClass () == PreferenceGroup_OnExpandButtonClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.PreferenceGroup+IOnExpandButtonClickListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public void onExpandButtonClick ()
	{
		n_onExpandButtonClick ();
	}

	private native void n_onExpandButtonClick ();

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
