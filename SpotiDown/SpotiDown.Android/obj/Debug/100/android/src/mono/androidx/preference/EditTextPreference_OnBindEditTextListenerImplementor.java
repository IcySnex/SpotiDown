package mono.androidx.preference;


public class EditTextPreference_OnBindEditTextListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.preference.EditTextPreference.OnBindEditTextListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBindEditText:(Landroid/widget/EditText;)V:GetOnBindEditText_Landroid_widget_EditText_Handler:AndroidX.Preference.EditTextPreference/IOnBindEditTextListenerInvoker, Xamarin.AndroidX.Preference\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Preference.EditTextPreference+IOnBindEditTextListenerImplementor, Xamarin.AndroidX.Preference", EditTextPreference_OnBindEditTextListenerImplementor.class, __md_methods);
	}


	public EditTextPreference_OnBindEditTextListenerImplementor ()
	{
		super ();
		if (getClass () == EditTextPreference_OnBindEditTextListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Preference.EditTextPreference+IOnBindEditTextListenerImplementor, Xamarin.AndroidX.Preference", "", this, new java.lang.Object[] {  });
	}


	public void onBindEditText (android.widget.EditText p0)
	{
		n_onBindEditText (p0);
	}

	private native void n_onBindEditText (android.widget.EditText p0);

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
