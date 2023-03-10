
CREATE TABLE public.users
(
    id integer NOT NULL,
    login text COLLATE pg_catalog."default" NOT NULL,
    name text COLLATE pg_catalog."default" NOT NULL,
    surname text COLLATE pg_catalog."default" NOT NULL,
    role text COLLATE pg_catalog."default" NOT NULL,
    birth date NOT NULL,
    hash bytea NOT NULL,
    expired timestamp without time zone NOT NULL,
    salt text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_users" PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.users
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_users_id"
    ON public.users USING btree
    (id)
    TABLESPACE pg_default;

INSERT INTO public.users(id, login, name, surname, role, birth, hash, expired, salt)
    VALUES (1, 'user', 'Иван', 'Иванов', 'Administrator', '1990-06-15', decode('KuE6XqU08OIuLWs01YjxgM5VPwFYSHK/czhK3lU8O4U=', 'base64'), '2023-03-11 11:22:03.977218', '2f6f714e-89ea-4dd8-89fb-2e1f95e4c09a');

INSERT INTO public.users(id, login, name, surname, role, birth, hash, expired, salt)
    VALUES (2, 'Petrov', 'Петр', 'Петров', 'RegisteredUser', '1995-07-19', decode('7UXPL5AFuVQbIgSckZ0i9JHhW28WKXQu6YLLZOH8Yws=', 'base64'), '2023-03-10 12:22:04.669666', '9023bf2e-e8dd-4008-8a7b-461da994a460');
