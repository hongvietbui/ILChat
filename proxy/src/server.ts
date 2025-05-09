import express from 'express';
import userRoutes from './routes/user';
import dotenv from 'dotenv';
dotenv.config();

const app = express();
app.use(express.json());

app.use('/user', userRoutes);

const PORT = process.env.PORT || 3001;
app.listen(PORT, () => console.log(`Proxy running at http://localhost:${PORT}`));