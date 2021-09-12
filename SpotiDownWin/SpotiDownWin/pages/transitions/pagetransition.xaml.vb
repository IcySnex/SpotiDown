Imports System.Windows.Media.Animation

Public Class pagetransition
    Inherits UserControl

    Private pages As Stack(Of UserControl) = New Stack(Of UserControl)()
    Public Property CurrentPage As UserControl
    Public Shared ReadOnly TransitionTypeProperty As DependencyProperty = DependencyProperty.Register("TransitionType", GetType(PageTransitionType), GetType(pagetransition), New PropertyMetadata(PageTransitionType.SlideAndFade))

    Public Property TransitionType As PageTransitionType
        Get
            Return CType(GetValue(TransitionTypeProperty), PageTransitionType)
        End Get
        Set(ByVal value As PageTransitionType)
            SetValue(TransitionTypeProperty, value)
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub ShowPage(ByVal newPage As UserControl)
        pages.Push(newPage)
        Call Task.Factory.StartNew(Sub() ShowNewPage())
    End Sub

    Private Sub ShowNewPage()
        Dispatcher.Invoke(CType(Sub()
                                    If contentPresenter.Content IsNot Nothing Then
                                        Dim oldPage As UserControl = TryCast(contentPresenter.Content, UserControl)

                                        If oldPage IsNot Nothing Then
                                            RemoveHandler oldPage.Loaded, AddressOf newPage_Loaded
                                            Me.UnloadPage(oldPage)
                                        End If
                                    Else
                                        ShowNextPage()
                                    End If
                                End Sub, Action))
    End Sub

    Private Sub ShowNextPage()
        Dim newPage As UserControl = pages.Pop()
        AddHandler newPage.Loaded, AddressOf newPage_Loaded
        contentPresenter.Content = newPage
    End Sub

    Private Sub UnloadPage(ByVal page As UserControl)
        Dim hidePage As Storyboard = TryCast(Resources(String.Format("{0}Out", TransitionType.ToString())), Storyboard).Clone()
        AddHandler hidePage.Completed, AddressOf hidePage_Completed
        hidePage.Begin(contentPresenter)
    End Sub

    Private Sub newPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim showNewPage As Storyboard = TryCast(Resources(String.Format("{0}In", TransitionType.ToString())), Storyboard)
        showNewPage.Begin(contentPresenter)
        CurrentPage = TryCast(sender, UserControl)
    End Sub

    Private Sub hidePage_Completed(ByVal sender As Object, ByVal e As EventArgs)
        contentPresenter.Content = Nothing
        ShowNextPage()
    End Sub
End Class