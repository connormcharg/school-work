import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";

let connection: HubConnection;

export const connectToHub = async () => {
    connection = new HubConnectionBuilder()
        .withUrl("http://localhost:5000/chesshub")
        .build();
    
    try {
        await connection.start();
        console.log("Connected to signalr hub");
    } catch (error) {
        console.error("Error connecting to signalr hub: ", error);
    }
}