CREATE TABLE public.work_shift
(
    index bigint NOT NULL,
    shift_open timestamp without time zone NOT NULL,
    shift_close timestamp without time zone NOT NULL,
    user_name text COLLATE pg_catalog."default" NOT NULL,
    user_id integer NOT NULL,
    cash integer NOT NULL,
    opened boolean NOT NULL,
    comments text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_work_shift" PRIMARY KEY (index)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.work_shift
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_work_shift_index"
    ON public.work_shift USING btree
    (index)
    TABLESPACE pg_default;

INSERT INTO public.work_shift(index, shift_open, shift_close, user_name, user_id, cash, opened, comments)
    VALUES ('1', '2023-01-25 09:00:00', '2023-01-25 15:00:00', 'И Иванов', 1, 10500, false, '');

INSERT INTO public.work_shift(index, shift_open, shift_close, user_name, user_id, cash, opened, comments)
    VALUES ('2', '2023-01-25 15:30:00', '2023-01-25 20:00:00', 'И Иванов', 1, 10500, false, '');

INSERT INTO public.work_shift(index, shift_open, shift_close, user_name, user_id, cash, opened, comments)
    VALUES ('3', '2023-01-26 09:30:00', '-infinity', 'И Иванов', 1, 10500, true, '');

INSERT INTO public.work_shift(index, shift_open, shift_close, user_name, user_id, cash, opened, comments)
    VALUES ('4', '2023-01-26 09:00:00', '-infinity', 'П Петров', 2, 11500, true, '');
