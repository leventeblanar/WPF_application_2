   M S B A M L   `   `   `  ����  AdatbazisApp     YourNamespace.FajtakWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Fajták��5   	   $	��600��6   $	��800��6%   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��10q�6;   .+�5      �� -   5        $��Hozzáadás��5      $	��100��6)   $��5q�65   6@   �� 5      -   5        $��Módosítás��5      $	��100��6)   $��5q�65   6@   �� 5      -   5        $��Törlés��5      $	��100��6&   $��5q�62   6=   5      &   System.Windows.Controls.DataGrid  5   
   -   5        $��FajtakDataGrid��5         AutoGenerateColumns$
 True��6)   $��1��6D   $��10q�6Q   �� 5   
   -   5        $��	FormPanel��5      $��2��6&   $��10q�63   $y�	Collapsed=�6?   .+�5      �� $��Fajta neve:��6   �� 5      -   5        $��NameTextBox��5      �� 5      $��	Leírás:��6   �� 5      -   5        $��DescriptionTextBox��5       �� AcceptsReturn$
 True��60   $	��100��6E   �� 5      -   5        $��Mentés��5      $	��100��6&   $��5q�62   $
��Left=�6=   6X   5      5       5!      