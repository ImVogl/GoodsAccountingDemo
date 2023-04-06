CREATE TABLE public.goods_states
(
    index serial NOT NULL,
    id uuid NOT NULL,
    write_off integer NOT NULL,
    receipt integer NOT NULL,
    storage integer NOT NULL,
    sold integer NOT NULL,
    retail_price real NOT NULL,
    whole_scale_price real NOT NULL,
    shift_identifier bigint NOT NULL,
    CONSTRAINT "PK_goods_states" PRIMARY KEY (index),
    CONSTRAINT "FK_goods_states_work_shift_shift_identifier" FOREIGN KEY (shift_identifier)
        REFERENCES public.work_shift (index) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.goods_states
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_goods_states_index"
    ON public.goods_states USING btree
    (index)
    TABLESPACE pg_default;

CREATE INDEX "IX_goods_states_shift_identifier"
    ON public.goods_states USING btree
    (shift_identifier)
    TABLESPACE pg_default;
