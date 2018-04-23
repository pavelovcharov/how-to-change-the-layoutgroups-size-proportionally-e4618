Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports DevExpress.Xpf.LayoutControl

Namespace DXSample
	Public Class ExtraLayoutControl
		Inherits DevExpress.Xpf.LayoutControl.LayoutControl

		Protected Overrides Function OnMeasure(ByVal constraint As Size) As Size
			If constraint.Width = 0 AndAlso constraint.Height = 0 Then
				Return MyBase.OnMeasure(constraint)
			End If

			Dim maxConstraint As New Size(Double.PositiveInfinity, Double.PositiveInfinity)

			Dim defaultConstraint As Size = constraint
			constraint = MyBase.OnMeasure(constraint)

			If Double.IsInfinity(defaultConstraint.Width) AndAlso Orientation = Orientation.Horizontal Then
				defaultConstraint.Width = constraint.Width
			End If
			If Double.IsInfinity(defaultConstraint.Height) AndAlso Orientation = Orientation.Vertical Then
				defaultConstraint.Height = constraint.Height
			End If

			Dim starCount As Double = 0

'			#Region "Create GroupLists"
			Dim listPixel As New List(Of ExtraLayoutGroup)()
			Dim listStar As New List(Of ExtraLayoutGroup)()

			For Each child As UIElement In Children
				If TypeOf child Is ExtraLayoutGroup Then
					If (CType(child, ExtraLayoutGroup)).LayoutItemSize.Value > 0 Then
						Select Case (CType(child, ExtraLayoutGroup)).LayoutItemSize.GridUnitType
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
				group.Measure(If((Orientation = Orientation.Horizontal), New Size(group.LayoutItemSize.Value, maxConstraint.Height), New Size(maxConstraint.Width, group.LayoutItemSize.Value)))
			Next group

'			#End Region

			constraint = MyBase.OnMeasure(defaultConstraint)
			If starCount = 0 Then
				Return constraint
			End If

			Dim starSize As Double = 0

'			#Region "Calc StarSize"

			Dim usedSize As Double = If((Orientation = Orientation.Horizontal), constraint.Width, constraint.Height)
			For Each group As ExtraLayoutGroup In listStar
				usedSize -= If((Orientation = Orientation.Horizontal), group.DesiredSize.Width, group.DesiredSize.Height)
			Next group

			Dim availableSize As Double = (If((Orientation = Orientation.Horizontal), defaultConstraint.Width, defaultConstraint.Height)) - usedSize
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
				group.Measure(If((Orientation = Orientation.Horizontal), New Size(group.Width, maxConstraint.Height), New Size(maxConstraint.Width, group.Height)))
			Next group

'			#End Region

			Return MyBase.OnMeasure(constraint)
		End Function
	End Class
End Namespace
