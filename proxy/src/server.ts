import express from 'express';
import dotenv from 'dotenv';
import router from './routes/proxy';
dotenv.config();

const app = express();
app.use(express.json());

app.use(router);

const PORT = process.env.PORT || 3001;
app.listen(PORT, () => console.log(`Proxy running at http://localhost:${PORT}`));