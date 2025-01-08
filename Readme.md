## Requirements
- .NET 8.0 SDK installed on your machine.

## How to Run
1. **Build the Project**  
   Use the following command to build the project:  
   ```bash
   dotnet build
   ```

2. **Navigate to the Binaries Folder**  
   After building the project, navigate to the folder containing the compiled binaries (usually located in the `bin` directory).

3. **Modify Data (Optional)**  
   If you want to adjust the seeded data, you can edit the following files:  
   - `bookings.json`
   - `hotels.json`

4. **Run the Application**  
   Run the executable file with the following command:  
   ```bash
   Guestline.exe
   ```

5. **Supported Commands**  
   You can interact with the application using these example commands:  
   - **Availability**: Check room availability for a hotel, date, and room type.  
     Example:  
     ```plaintext
     Availability(H1, 20250906, SGL)
     ```
   - **Search**: Search for hotel availability based on a range of days and room type.  
     Example:  
     ```plaintext
     Search(H1, 365, SGL)
     ```