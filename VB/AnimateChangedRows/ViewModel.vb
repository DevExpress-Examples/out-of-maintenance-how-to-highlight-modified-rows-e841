Imports System
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Threading

Namespace AnimateChangedRows

    Public Class ViewModel

        Public Property List As ObservableCollection(Of TestData)

        Private timer As DispatcherTimer = New DispatcherTimer()

        Private random As Random = New Random()

        Public Sub New()
            List = New ObservableCollection(Of TestData)()
            PopulateData()
            AddHandler timer.Tick, New EventHandler(AddressOf timer_Tick)
            timer.Interval = TimeSpan.FromMilliseconds(100)
            timer.IsEnabled = True
        End Sub

        Private Sub PopulateData()
            For i As Integer = 0 To 30 - 1
                Dim t = New TestData(i, "Element" & i.ToString())
                List.Add(t)
            Next
        End Sub

        Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
            Dim rowHandle As Integer = random.Next(List.Count)
            List(rowHandle).Number = random.Next(1000)
        End Sub
    End Class

    Public Class TestData
        Implements INotifyPropertyChanged

        Private numberField As Integer

        Private textField As String

        Public Sub New(ByVal number As Integer, ByVal text As String)
            numberField = number
            textField = text
        End Sub

        Public Property Number As Integer
            Get
                Return numberField
            End Get

            Set(ByVal value As Integer)
                If numberField = value Then Return
                numberField = value
                NotifyChanged("Number")
            End Set
        End Property

        Public Property Text As String
            Get
                Return textField
            End Get

            Set(ByVal value As String)
                If Equals(textField, value) Then Return
                textField = value
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
End Namespace
