Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Windows.Threading

Namespace AnimateChangedRows
    Public Class ViewModel

        Public Property List() As ObservableCollection(Of TestData)
        Private timer As New DispatcherTimer()
        Private random As New Random()


        Public Sub New()
            List = New ObservableCollection(Of TestData)()
            PopulateData()
            AddHandler timer.Tick, AddressOf timer_Tick
            timer.Interval = TimeSpan.FromMilliseconds(100)
            timer.IsEnabled = True
        End Sub

        Private Sub PopulateData()
            For i As Integer = 0 To 29
                Dim t = New TestData(i, "Element" & i.ToString())
                List.Add(t)
            Next i
        End Sub

        Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
            Dim rowHandle As Integer = random.Next(List.Count)
            List(rowHandle).Number = random.Next(1000)
        End Sub
    End Class

    Public Class TestData
        Implements INotifyPropertyChanged


        Private number_Renamed As Integer

        Private text_Renamed As String

        Public Sub New(ByVal number As Integer, ByVal text As String)
            Me.number_Renamed = number
            Me.text_Renamed = text
        End Sub



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
End Namespace
