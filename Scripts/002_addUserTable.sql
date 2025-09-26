CREATE TABLE IF NOT EXISTS public.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    sexo VARCHAR(50),
    data_nascimento DATE NOT NULL,
    naturalidade VARCHAR(255),
    nacionalidade VARCHAR(255),
    cpf VARCHAR(14) NOT NULL UNIQUE,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW()
);
