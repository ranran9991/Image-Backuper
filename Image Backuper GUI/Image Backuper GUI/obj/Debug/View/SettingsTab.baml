   M S B A M L   `   `   `  ����M  IImage Backuper GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:Image_Backuper_GUI.ViewImage_Backuper_GUI.View  5   	   )    #Image_Backuper_GUI.View.SettingsTab   5   
   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:Image_Backuper_GUI.View     5   	   $��Settings��5   	   $��30��5	   	   $��Center=�5
   	   .��5      �� $��3q�6   .+�5   
   �� $��3q�6   
����  �$��30(�61   $��Center=�6?   $
��Bold"�6\   
Settings6y   &   System.Windows.Controls.DataGrid  5   
       HeadersVisibility$
  None=�6      MinRowHeight$ 30Q�6,      AutoGenerateColumns .     
IsReadOnly . $��Center=�6�   ��6>   �� *data+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   ��  �� Path$	 key�  5       6%   �� $ value�5      �� 5   
   -   5        $��remove��5      $��Remove��5      $��30��5      ��5      �� *RemoveCommand+�� 5   
   -   5        $��dirList��5       �� SelectionMode$ Single=�5      ��. ! �� IsDeferredScrollingEnabled . $��Visible=�5      $	��180��5      	 �� SelectedIndex	 5      �� *handlerIndex+
 �� Mode$
 TwoWay=���5      �� *directoryList+��5      �� 5       .R�5!      �� .+�5"      �� $��30��65   ��6$   �� 65   5#      5$      5&      5'      5(      