CREATE TABLE public.work_shift
(
    index serial NOT NULL,
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
