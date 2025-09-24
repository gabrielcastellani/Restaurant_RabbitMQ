This project was developed using .NET 8 and leverages RabbitMQ for asynchronous communication.
The message broker is hosted in the cloud using CloudAMQP with the free plan, which allows easy setup and management without the need to install RabbitMQ locally.

## Messaging Architecture – Restaurant Order System

<img width="1095" height="406" alt="image" src="https://github.com/user-attachments/assets/e381ee77-616d-417c-953f-a71dcb953c95" />

This diagram represents the asynchronous messaging flow between services in the restaurant order system, using RabbitMQ (or a compatible message broker).

### Components
- **OrderService (Producer):** Responsible for receiving new orders and publishing them to the Exchange.
  - Publishes messages with the routing key order.placed.
- **Exchange:** A topic (or direct) exchange that routes messages to queues based on the routing key
  - Bindings:
    - order.placed → routes to queue: orders.
    - order.ready → routes to queue: kitchen.
- **Queues:**
  - **orders**: holds newly created orders.
  - **kitchen**: receives ready orders for the kitchen workflow.
  - **delivery**: receives ready orders for the delivery workflow.
- **KitchenService (Consumer/Producer)**
  - Consumes messages from queue: orders (new orders).
  - Processes them and, when done, publishes a new message with the routing key order.ready, signaling that the order is ready
- **DeliveryService (Consumer)**
  - Consumes messages from queue: kitchen
  - Responsible for managing order delivery logistics.
 
### Message Flow
1. **OrderService** creates an order and publishes it to the exchange using order.placed.
2. The message is routed to the queue: orders.
3. KitchenService consumes from queue: orders, processes the order, and publishes a new message with order.ready.
4. The message is routed to kitchen.
5. DeliveryService consumes from queue: kitchen to manage the delivery process

### Benefits of this Architecture
- **Loose Coupling:** OrderService does not need to know about KitchenService or DeliveryService.
- **Scalability:** Each consumer can be scaled horizontally based on demand.
- **Resilience:** Messages are stored in queues until they are processed, avoiding data loss during temporary outages.
