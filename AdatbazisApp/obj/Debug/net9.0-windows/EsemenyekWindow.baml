   M S B A M L   `   `   `  ����  AdatbazisApp#    YourNamespace.EsemenyekWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
Események��5   	   $	��600��6   $	��800��6(   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��10q�6;   .+�5      �� -   5        $��Hozzáadás��5      $	��100��6)   $��5q�65   6@   �� 5      -   5        $��Módosítás��5      $	��100��6)   $��5q�65   6@   �� 5      -   5        $��Törlés��5      $	��100��6&   $��5q�62   6=   5      &   System.Windows.Controls.DataGrid  5   
   -   5        $��EsemenyekDataGrid��5         AutoGenerateColumns$
 True��6,   $��1��6G   $��10q�6T   �� 5   
   -   5        $��	FormPanel��5      $��2��6&   $��10q�63   $y�	Collapsed=�6?   .+�5      �� $��Kutya:��6   �� 5      -   5        $��KutyaComboBox��5      6,   �� 5      $��Gazda:��6   �� 5      -   5        $��GazdaComboBox��5      ${�False��6,   �� 5      $��Esemény Dátuma:��6   (  "System.Windows.Controls.DatePicker  5      -   5        $��EsemenyDatePicker��5      �� 5      $��	Leírás:��6   �� 5      -	   5        $��LeirasTextBox��5      $	��300��6+   $	��100��67    �� TextWrapping$
 Wrap=�6D    �� AcceptsReturn$
 True��6X   �� 5!      -
   5        $��Mentés��5!      $	��100��6&   $��5q�62   $
��Left=�6=   6X   5"      5#      5$      