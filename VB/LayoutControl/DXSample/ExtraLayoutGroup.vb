Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports DevExpress.Xpf.LayoutControl
Imports System.Windows.Controls

Namespace DXSample
	Public Class ExtraLayoutGroup
		Inherits LayoutGroup
		Public Property LayoutItemSize() As GridLength
			Get
				Return CType(GetValue(LayoutItemSizeProperty), GridLength)
			End Get
			Set(ByVal value As GridLength)
				SetValue(LayoutItemSizeProperty, value)
			End Set
		End Property
		Public Shared ReadOnly LayoutItemSizeProperty As DependencyProperty = DependencyProperty.Register("LayoutItemSize", GetType(GridLength), GetType(ExtraLayoutGroup), New PropertyMetadata(New GridLength(0, GridUnitType.Star)))

		Protected Overrides Function OnMeasure(ByVal constraint As Size) As Size
			If constraint.Width = 0 AndAlso constraint.Height = 0 Then
				Return MyBase.OnMeasure(constraint)
			End If

			Dim maxConstraint As New Size(Double.PositiveInfinity, Double.PositiveInfinity)

			Dim defaultConstraint As Size = constraint
			constraint = MyBase.OnMeasure(constraint)
			Dim minConstraint As New Size(Math.Min(defaultConstraint.Width, constraint.Width), Math.Min(defaultConstraint.Height, constraint.Height))

			Dim starCount As Double = 0

'			#Region "Create GroupLists"
			Dim listPixel As New List(Of ExtraLayoutGroup)()
			Dim listStar As New List(Of ExtraLayoutGroup)()

			For Each child As UIElement In Children
				If TypeOf child Is ExtraLayoutGroup Then
					Dim group As ExtraLayoutGroup = CType(child, ExtraLayoutGroup)

					If TypeOf Parent Is ExtraLayoutControl Then
						Select Case (CType(Parent, ExtraLayoutControl)).Orientation
							Case Orientation.Horizontal
								CType(child, FrameworkElement).HorizontalAlignment = HorizontalAlignment.Stretch
							Case Orientation.Vertical
								CType(child, FrameworkElement).VerticalAlignment = VerticalAlignment.Stretch
						End Select
					End If
					If TypeOf Parent Is ExtraLayoutGroup Then
						Select Case (CType(Parent, ExtraLayoutGroup)).Orientation
							Case Orientation.Horizontal
								CType(child, FrameworkElement).HorizontalAlignment = HorizontalAlignment.Stretch
							Case Orientation.Vertical
								CType(child, FrameworkElement).VerticalAlignment = VerticalAlignment.Stretch
						End Select
					End If

					If group.LayoutItemSize.Value > 0 Then
						Select Case group.LayoutItemSize.GridUnitType
							Case GridUnitType.Pixel
								listPixel.Add(CType(child, ExtraLayoutGroup))
							Case GridUnitType.Star
								listStar.Add(CType(child, ExtraLayoutGroup))
								starCount += (CType(child, ExtraLayoutGroup)).LayoutItemSize.Value
						End Select
					End If
				End If
			Next child

			If listPixel.Count = 0 AndAlso listStar.Count = 0 Then
				Return constraint
			End If

'			#End Region

'			#Region "Resize Pixeled"

			For Each group As ExtraLayoutGroup In listPixel
				If Orientation = Orientation.Horizontal Then
					group.Width = group.LayoutItemSize.Value
				Else
					group.Height = group.LayoutItemSize.Value
				End If

				group.InvalidateMeasure()
				group.Measure(minConstraint)
			Next group

'			#End Region

			constraint = MyBase.OnMeasure(minConstraint)
			minConstraint.Width = Math.Min(minConstraint.Width, constraint.Width)
			minConstraint.Height = Math.Min(minConstraint.Height, constraint.Height)

			If starCount = 0 Then
				Return minConstraint
			End If

			Dim starSize As Double = 0

'			#Region "Calc StarSize"

			Dim usedSize As Double = If((Orientation = Orientation.Horizontal), constraint.Width, constraint.Height)
			For Each group As ExtraLayoutGroup In listStar
				usedSize -= If((Orientation = Orientation.Horizontal), group.DesiredSize.Width, group.DesiredSize.Height)
			Next group

			Dim availableSize As Double = (If((Orientation = Orientation.Horizontal), minConstraint.Width, minConstraint.Height)) - usedSize
			starSize = availableSize / starCount

'			#End Region

'			#Region "Resize Starred"

			For Each group As ExtraLayoutGroup In listStar
				If Orientation = Orientation.Horizontal Then
					group.Width = group.LayoutItemSize.Value * starSize
				Else
					group.Height = group.LayoutItemSize.Value * starSize
				End If

				group.InvalidateMeasure()
				group.Measure(If((Orientation = Orientation.Horizontal), New Size(group.Width, minConstraint.Height), New Size(minConstraint.Width, group.Height)))
			Next group

'			#End Region

			Return MyBase.OnMeasure(minConstraint)
		End Function

	End Class
End Namespace
