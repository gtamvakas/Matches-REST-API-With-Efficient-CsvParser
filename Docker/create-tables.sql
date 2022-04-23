
CREATE SCHEMA exercise AUTHORIZATION postgres;

CREATE SEQUENCE exercise.stats_stats_id_seq;

ALTER SEQUENCE exercise.stats_stats_id_seq
    OWNER TO postgres;

CREATE TABLE exercise.matches
(
    id bigint NOT NULL,
    division character(2) COLLATE pg_catalog."default",
    date date,
    home_team character varying COLLATE pg_catalog."default" NOT NULL,
    away_team character varying COLLATE pg_catalog."default" NOT NULL,
    ft_home_goals smallint,
    ft_away_goals smallint,
    ht_home_goals smallint,
    ht_away_goals smallint,
    full_time_result character(1) COLLATE pg_catalog."default" NOT NULL,
    half_time_result character(1) COLLATE pg_catalog."default",
    CONSTRAINT matches_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE exercise.matches
    OWNER to postgres;


CREATE TABLE exercise.stats
(
    stats_id integer NOT NULL DEFAULT nextval('exercise.stats_stats_id_seq'::regclass),
    match_id bigint,
    attendance integer,
    referee text COLLATE pg_catalog."default",
    CONSTRAINT stats_pkey PRIMARY KEY (stats_id),
    CONSTRAINT constraint_fkey FOREIGN KEY (match_id)
        REFERENCES exercise.matches (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE exercise.stats
    OWNER to postgres;