Functionalities implemented are,
	---------Case-------------------------##--------------------How to----------------
	1. Add new stock exchange	-				1. Type in (text box located at the top right) the exchange short symbol
													used by google.
												2. Select from the dropdown (same text box located at the top right) 
													to select an exchange. 

	2. Delete the current (slected) exchange	1. Select an exchange and click delete button at top right

	3. Add a stock								1. Type in (text box located just above the table) the stock symbol 
													for the exchange used by google.
												2. Select from the dropdown (same text box located just above the table) 
													to select the stock. 

	4. Delete one or more stocks				1. Check one or more items from the table and click delete.

	5. Configuration is saved					1. Added exchanges and stocks are saved and will be available next time.

	6. Minimize to tray							1. Click on the tray icon to minimize/maximize to/from tray.
	
-----------------------------------------------------------------------------------------------------------------------------

Implementation details:							1. Used MVVM for add/delete/retrieve stocks info.
												2. Used code behind for add/delete stock exchange.
												3. Log manager to log errors.
												4. Error information panel at bottom of screen.
												4. Used Nunit for unit testing.

												