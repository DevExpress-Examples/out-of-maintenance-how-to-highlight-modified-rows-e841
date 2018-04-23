Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports DevExpress.Xpf.Grid

Namespace AnimateChangedRows
	Partial Public Class Window1
		Inherits Window
		Private list As New BindingList(Of TestData)()

		Private animationElements As New Dictionary(Of Integer, AnimationElement)()

		Private timer As New DispatcherTimer()

		Public Sub New()
			InitializeComponent()

			For i As Integer = 0 To 99
				list.Add(New TestData() With {.Text = "Element" & i.ToString(), .Number = i})
			Next i
			grid.DataSource = list

			AddHandler timer.Tick, AddressOf timer_Tick
			timer.Interval = TimeSpan.FromMilliseconds(100)
			timer.IsEnabled = True
		End Sub

		Private randow As New Random()

		Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
			Dim rowHandle As Integer = grid.GetRowHandleByListIndex(randow.Next(list.Count))
			grid.SetCellValue(rowHandle, grid.Columns(1), randow.Next(1000))
		End Sub

		Private Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
			If e.Column.FieldName = "AnimationElement" Then
				e.Value = GetAnimationElement(e.ListSourceRowIndex)
			End If
		End Sub

		Private Function GetAnimationElement(ByVal listIndex As Integer) As AnimationElement
            Dim element As AnimationElement = Nothing
            If (Not animationElements.TryGetValue(listIndex, element)) Then
                element = New AnimationElement()
                animationElements(listIndex) = element
            End If
			Return element
		End Function

		Private Sub view_CellValueChanged(ByVal sender As Object, ByVal e As CellValueEventArgs)
			GetAnimationElement(grid.GetRowListIndex(e.RowHandle)).StartAnimation()
		End Sub
	End Class

	Public Class TestData
		Implements INotifyPropertyChanged
		Private number_Renamed As Integer
		Private text_Renamed As String
		Public Property Number() As Integer
			Get
				Return number_Renamed
			End Get
			Set(ByVal value As Integer)
				If number_Renamed = value Then
					Return
				End If
				number_Renamed = value
				NotifyChanged("Number")
			End Set
		End Property
		Public Property Text() As String
			Get
				Return text_Renamed
			End Get
			Set(ByVal value As String)
				If text_Renamed = value Then
					Return
				End If
				text_Renamed = value
				NotifyChanged("Text")
			End Set
		End Property
		Private Sub NotifyChanged(ByVal propertyName As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End Sub

		#Region "INotifyPropertyChanged Members"
		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
		#End Region
	End Class

	Public Class AnimationElement
		Inherits FrameworkContentElement
        Public Shared ReadOnly ColorProperty As DependencyProperty = DependencyProperty.Register("Color", GetType(Color), GetType(AnimationElement), New PropertyMetadata(Colors.White, AddressOf OnChanged))

		Private Shared Sub OnChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
		End Sub
		Public Property Color() As Color
			Get
				Return CType(GetValue(ColorProperty), Color)
			End Get
			Set(ByVal value As Color)
				SetValue(ColorProperty, value)
			End Set
		End Property
		Public Sub StartAnimation()
			Dim animation As New ColorAnimationUsingKeyFrames()
			animation.KeyFrames.Add(New DiscreteColorKeyFrame(Color.FromArgb(255, 255, 255, 255), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))))
			animation.KeyFrames.Add(New LinearColorKeyFrame(Color.FromArgb(255, 200, 200, 200), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))))
			animation.KeyFrames.Add(New LinearColorKeyFrame(Color.FromArgb(255, 255, 255, 255), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))))

			Me.BeginAnimation(AnimationElement.ColorProperty, animation, HandoffBehavior.SnapshotAndReplace)
		End Sub
	End Class

	Public Class ColorToBrushConverter
		Inherits MarkupExtension
		Implements IValueConverter
		Private Function IValueConverter_Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
			Return New SolidColorBrush(CType(value, Color))
		End Function
		Private Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
			Return value
		End Function
		Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
			Return Me
		End Function
	End Class
End Namespace
